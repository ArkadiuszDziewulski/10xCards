-- migration: initial schema setup for 10xcards
-- description: creates profiles, decks, flashcards, and generation_events tables with rls and indexing.
-- affected tables: public.profiles, public.decks, public.flashcards, public.generation_events

-- ############################################################################
-- 1. extensions and helper functions
-- ############################################################################

-- generic function to update updated_at timestamp
create or replace function public.handle_updated_at()
returns trigger as $$
begin
  new.updated_at = now();
  return new;
end;
$$ language plpgsql;

-- ############################################################################
-- 2. tables creation
-- ############################################################################

-- profiles table (extends auth.users)
-- stores additional user metadata and serves as a fk target for other tables
create table if not exists public.profiles (
  id uuid primary key references auth.users(id) on delete cascade,
  created_at timestamptz default now() not null,
  updated_at timestamptz default now() not null
);

comment on table public.profiles is 'user profiles linked to supabase auth users.';

-- decks table (user collections)
-- unique constraint on (user_id, name) prevents duplicate deck names per user
create table if not exists public.decks (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references public.profiles(id) on delete cascade,
  name text not null,
  created_at timestamptz default now() not null,
  updated_at timestamptz default now() not null,
  unique (user_id, name)
);

comment on table public.decks is 'collections of flashcards organized by users.';

-- flashcards table (content and srs metadata)
-- denormalized user_id added for efficient rls policies
create table if not exists public.flashcards (
  id uuid primary key default gen_random_uuid(),
  deck_id uuid not null references public.decks(id) on delete cascade,
  user_id uuid not null references public.profiles(id) on delete cascade,
  front text not null,
  back text not null,
  status text not null default 'active' check (status in ('active', 'draft')),
  next_review_at timestamptz,
  interval integer not null default 0,
  ease_factor real not null default 2.5,
  repetition_count integer not null default 0,
  created_at timestamptz default now() not null,
  updated_at timestamptz default now() not null
);

comment on table public.flashcards is 'individual flashcards with spaced repetition system metadata.';

-- generation_events table (ai generation history)
-- tracks ai usage metrics without storing the actual generated content
create table if not exists public.generation_events (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references public.profiles(id) on delete cascade,
  target_deck_id uuid references public.decks(id) on delete set null,
  input_length integer not null,
  total_generated_count integer not null,
  accepted_count integer not null default 0,
  created_at timestamptz default now() not null
);

comment on table public.generation_events is 'logs of ai generation requests for analytics and monitoring.';

-- ############################################################################
-- 3. performance indexes
-- ############################################################################

-- index for deck ownership lookups
create index if not exists decks_user_id_idx on public.decks (user_id);

-- index for sorting decks by creation date
create index if not exists decks_created_at_idx on public.decks (created_at desc);

-- index for retrieving flashcards within a deck
create index if not exists flashcards_deck_id_idx on public.flashcards (deck_id);

-- index for flashcard ownership lookups (rls optimization)
create index if not exists flashcards_user_id_idx on public.flashcards (user_id);

-- index for srs scheduling - critical for "cards due for review" queries
create index if not exists flashcards_next_review_idx on public.flashcards (next_review_at) where next_review_at is not null;

-- ############################################################################
-- 4. triggers
-- ############################################################################

-- trigger function to create profile on user signup
create or replace function public.handle_new_user()
returns trigger as $$
begin
  insert into public.profiles (id)
  values (new.id);
  return new;
end;
$$ language plpgsql security definer;

-- trigger setup on auth.users for automatic profile creation
create or replace trigger on_auth_user_created
  after insert on auth.users
  for each row execute procedure public.handle_new_user();

-- updated_at triggers to maintain timestamps across tables
create trigger set_profiles_updated_at
  before update on public.profiles
  for each row execute procedure public.handle_updated_at();

create trigger set_decks_updated_at
  before update on public.decks
  for each row execute procedure public.handle_updated_at();

create trigger set_flashcards_updated_at
  before update on public.flashcards
  for each row execute procedure public.handle_updated_at();

-- ############################################################################
-- 5. row level security (rls)
-- ############################################################################

alter table public.profiles enable row level security;
alter table public.decks enable row level security;
alter table public.flashcards enable row level security;
alter table public.generation_events enable row level security;

-- profiles policies: users can manage their own identity
create policy "profiles_select_authenticated" 
on public.profiles for select 
to authenticated 
using (auth.uid() = id);

create policy "profiles_update_authenticated" 
on public.profiles for update 
to authenticated 
using (auth.uid() = id)
with check (auth.uid() = id);

-- decks policies: users can manage their own collections
create policy "decks_select_authenticated" 
on public.decks for select 
to authenticated 
using (auth.uid() = user_id);

create policy "decks_insert_authenticated" 
on public.decks for insert 
to authenticated 
with check (auth.uid() = user_id);

create policy "decks_update_authenticated" 
on public.decks for update 
to authenticated 
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

create policy "decks_delete_authenticated" 
on public.decks for delete 
to authenticated 
using (auth.uid() = user_id);

-- flashcards policies: users can manage their own flashcards
create policy "flashcards_select_authenticated" 
on public.flashcards for select 
to authenticated 
using (auth.uid() = user_id);

create policy "flashcards_insert_authenticated" 
on public.flashcards for insert 
to authenticated 
with check (auth.uid() = user_id);

create policy "flashcards_update_authenticated" 
on public.flashcards for update 
to authenticated 
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

create policy "flashcards_delete_authenticated" 
on public.flashcards for delete 
to authenticated 
using (auth.uid() = user_id);

-- generation_events policies: users can view and log their ai usage
create policy "generation_events_select_authenticated" 
on public.generation_events for select 
to authenticated 
using (auth.uid() = user_id);

create policy "generation_events_insert_authenticated" 
on public.generation_events for insert 
to authenticated 
with check (auth.uid() = user_id);
