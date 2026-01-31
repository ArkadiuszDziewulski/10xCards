CREATE POLICY "Allow insert for authenticated users" 
ON generation_events 
FOR INSERT 
TO authenticated 
WITH CHECK (auth.uid() = user_id);