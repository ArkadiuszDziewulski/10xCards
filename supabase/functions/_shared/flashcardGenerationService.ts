export interface GenerateFlashcardsRequest {
    text: string;
    amount: number;
}

export interface FlashcardSuggestionDto {
    front: string;
    back: string;
}

export interface OpenRouterConfig {
    url: string;
    key: string;
    model: string;
    timeoutMs: number;
}

export class FlashcardGenerationError extends Error {
    constructor(message: string, public readonly status: number) {
        super(message);
    }
}

const MinTextLength = 1000;
const MaxTextLength = 10000;

export function validateGenerateRequest(payload: GenerateFlashcardsRequest): void {
    if (!payload || typeof payload !== "object") {
        throw new FlashcardGenerationError("Request payload is required.", 400);
    }

    if (typeof payload.text !== "string") {
        throw new FlashcardGenerationError("Text must be a string.", 400);
    }

    const text = payload.text.trim();
    if (!text) {
        throw new FlashcardGenerationError("Text is required.", 400);
    }

    if (text.length < MinTextLength || text.length > MaxTextLength) {
        throw new FlashcardGenerationError("Text length must be between 1000 and 10000 characters.", 400);
    }

    if (!Number.isFinite(payload.amount) || payload.amount <= 0) {
        throw new FlashcardGenerationError("Amount must be greater than zero.", 400);
    }
}

export async function generateFlashcards(
    payload: GenerateFlashcardsRequest,
    config: OpenRouterConfig,
): Promise<FlashcardSuggestionDto[]> {
    if (!config.url || !config.key) {
        throw new FlashcardGenerationError("OpenRouter configuration is missing.", 500);
    }

    const systemMessage =
        "You are an assistant that generates flashcards. Respond only with valid JSON matching the provided schema.";
    const userMessage = `Generate ${payload.amount} concise flashcards based on the following text:\n${payload.text}`;

    const responseFormat = {
        type: "json_schema",
        json_schema: {
            name: "flashcards",
            schema: {
                type: "object",
                properties: {
                    flashcards: {
                        type: "array",
                        items: {
                            type: "object",
                            properties: {
                                front: { type: "string" },
                                back: { type: "string" },
                            },
                            required: ["front", "back"],
                        },
                    },
                },
                required: ["flashcards"],
            },
        },
    };

    const requestBody = {
        model: config.model,
        messages: [
            { role: "system", content: systemMessage },
            { role: "user", content: userMessage },
        ],
        response_format: responseFormat,
        temperature: 0.2,
        max_tokens: 1500,
    };

    const timeoutController = new AbortController();
    const timeout = setTimeout(() => timeoutController.abort(), config.timeoutMs);

    try {
        const response = await fetch(`${config.url.replace(/\/$/, "")}/chat/completions`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${config.key}`,
            },
            body: JSON.stringify(requestBody),
            signal: timeoutController.signal,
        });

        if (!response.ok) {
            throw new FlashcardGenerationError("OpenRouter request failed.", 500);
        }

        const data = await response.json();
        const content = data?.choices?.[0]?.message?.content;
        if (typeof content !== "string") {
            throw new FlashcardGenerationError("OpenRouter response is invalid.", 500);
        }

        let parsed: { flashcards?: FlashcardSuggestionDto[] };
        try {
            parsed = JSON.parse(content);
        } catch {
            throw new FlashcardGenerationError("OpenRouter response is not valid JSON.", 500);
        }

        if (!Array.isArray(parsed.flashcards)) {
            throw new FlashcardGenerationError("OpenRouter response is missing flashcards.", 500);
        }

        const normalized = parsed.flashcards
            .filter((item) => item && typeof item.front === "string" && typeof item.back === "string")
            .map((item) => ({
                front: item.front.trim(),
                back: item.back.trim(),
            }))
            .filter((item) => item.front.length > 0 && item.back.length > 0);

        if (normalized.length === 0) {
            throw new FlashcardGenerationError("OpenRouter returned no flashcards.", 500);
        }

        return normalized.slice(0, payload.amount);
    } catch (error) {
        if (error instanceof FlashcardGenerationError) {
            throw error;
        }

        if (error instanceof DOMException && error.name === "AbortError") {
            throw new FlashcardGenerationError("OpenRouter request timed out.", 500);
        }

        throw new FlashcardGenerationError("OpenRouter request failed.", 500);
    } finally {
        clearTimeout(timeout);
    }
}
