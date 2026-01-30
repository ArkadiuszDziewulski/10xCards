import { serve } from "https://deno.land/std@0.224.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2";
import type { TablesInsert } from "../../../10xCards/ai/Model.types.ts";
import {
    FlashcardGenerationError,
    generateFlashcards,
    validateGenerateRequest,
    type GenerateFlashcardsRequest,
} from "../_shared/flashcardGenerationService.ts";

const supabaseUrl = Deno.env.get("SUPABASE_URL") ?? "";
const supabaseKey = Deno.env.get("SUPABASE_ANON_KEY") ?? "";
const openRouterUrl = Deno.env.get("OPENROUTER_URL") ?? "https://openrouter.ai/api/v1";
const openRouterKey = Deno.env.get("OPENROUTER_KEY") ?? "";
const openRouterModel = Deno.env.get("OPENROUTER_MODEL") ?? "openai/gpt-4o-mini";
const openRouterTimeoutMs = Number(Deno.env.get("OPENROUTER_TIMEOUT_MS") ?? "30000");

const jsonHeaders = { "Content-Type": "application/json" };

serve(async (request) => {
    if (request.method !== "POST") {
        return new Response(JSON.stringify({ message: "Method not allowed." }), {
            status: 405,
            headers: jsonHeaders,
        });
    }

    if (!supabaseUrl || !supabaseKey) {
        return new Response(JSON.stringify({ message: "Supabase configuration is missing." }), {
            status: 500,
            headers: jsonHeaders,
        });
    }

    if (!openRouterKey) {
        return new Response(JSON.stringify({ message: "OpenRouter configuration is missing." }), {
            status: 500,
            headers: jsonHeaders,
        });
    }

    const authHeader = request.headers.get("Authorization") ?? "";
    if (!authHeader.startsWith("Bearer ")) {
        return new Response(JSON.stringify({ message: "Authorization token is required." }), {
            status: 401,
            headers: jsonHeaders,
        });
    }

    const token = authHeader.replace("Bearer", "").trim();
    if (!token) {
        return new Response(JSON.stringify({ message: "Authorization token is required." }), {
            status: 401,
            headers: jsonHeaders,
        });
    }

    let payload: GenerateFlashcardsRequest;
    try {
        payload = await request.json();
    } catch {
        return new Response(JSON.stringify({ message: "Invalid JSON payload." }), {
            status: 400,
            headers: jsonHeaders,
        });
    }

    try {
        validateGenerateRequest(payload);
    } catch (error) {
        if (error instanceof FlashcardGenerationError) {
            return new Response(JSON.stringify({ message: error.message }), {
                status: error.status,
                headers: jsonHeaders,
            });
        }

        return new Response(JSON.stringify({ message: "Invalid request." }), {
            status: 400,
            headers: jsonHeaders,
        });
    }

    const supabaseClient = createClient(supabaseUrl, supabaseKey, {
        global: { headers: { Authorization: `Bearer ${token}` } },
    });

    const { data: userData, error: userError } = await supabaseClient.auth.getUser(token);
    if (userError || !userData?.user) {
        return new Response(JSON.stringify({ message: "Unauthorized." }), {
            status: 401,
            headers: jsonHeaders,
        });
    }

    try {
        const flashcards = await generateFlashcards(payload, {
            url: openRouterUrl,
            key: openRouterKey,
            model: openRouterModel,
            timeoutMs: Number.isFinite(openRouterTimeoutMs) ? openRouterTimeoutMs : 30000,
        });

        const generationEvent: TablesInsert<"generation_events"> = {
            user_id: userData.user.id,
            input_length: payload.text.length,
            total_generated_count: flashcards.length,
            accepted_count: 0,
        };

        const { data: eventData, error: eventError } = await supabaseClient
            .from("generation_events")
            .insert(generationEvent)
            .select("id")
            .single();

        if (eventError || !eventData) {
            return new Response(JSON.stringify({ message: "Failed to save generation event." }), {
                status: 500,
                headers: jsonHeaders,
            });
        }

        return new Response(
            JSON.stringify({
                success: true,
                flashcards,
                generationId: eventData.id,
            }),
            { status: 201, headers: jsonHeaders },
        );
    } catch (error) {
        const message =
            error instanceof FlashcardGenerationError
                ? error.message
                : "Unexpected error while generating flashcards.";
        const status = error instanceof FlashcardGenerationError ? error.status : 500;

        return new Response(JSON.stringify({ message }), {
            status,
            headers: jsonHeaders,
        });
    }
});
