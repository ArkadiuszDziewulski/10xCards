### C# Sign in with OAuth provider and custom scopes

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithoauth

Performs OAuth sign-in with a third-party provider while requesting specific permissions (scopes). After the user completes the authentication flow and is redirected, this snippet also demonstrates how to retrieve the user's session from the redirected URL.

```C#
var signInUrl = supabase.Auth.SignIn(Provider.Github, 'repo gist notifications');

// after user comes back from signin flow
var session = supabase.Auth.GetSessionFromUrl(REDIRECTED_URI);
```

--------------------------------

### C# Sign in with a third-party OAuth provider

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithoauth

Initiates the OAuth sign-in process for a user using a specified third-party provider, such as GitHub. This method returns a URL to which the user should be redirected to complete the authentication flow.

```C#
var signInUrl = supabase.Auth.SignIn(Provider.Github);
```

--------------------------------

### Send Magic Link using C# Supabase Auth

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithotp

Demonstrates how to send a magic link to a user's email address for passwordless sign-in, optionally specifying a redirect URL. This method is part of the Supabase Auth client.

```C#
var options = new SignInOptions { RedirectTo = "http://myredirect.example" };
var didSendMagicLink = await supabase.Auth.SendMagicLink("joseph@supabase.io", options);
```

--------------------------------

### Listen to Supabase authentication state changes in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-onauthstatechange

This C# code snippet demonstrates how to add a listener for Supabase authentication state changes, allowing the application to react to various events such as user sign-in, sign-out, updates, password recovery, and token refreshes.

```C#
supabase.Auth.AddStateChangedListener((sender, changed) =>
{
    switch (changed)
    {
        case AuthState.SignedIn:
            break;
        case AuthState.SignedOut:
            break;
        case AuthState.UserUpdated:
            break;
        case AuthState.PasswordRecovery:
            break;
        case AuthState.TokenRefreshed:
            break;
    }
});
```

--------------------------------

### Send Magic Link for Sign-In

Source: https://supabase.com/docs/reference/csharp/introduction/index

Sends a magic link to a user's email address for passwordless authentication. The link's destination can be configured via SITE_URL or additional redirect URLs.

```csharp
var options = new SignInOptions { RedirectTo = "http://myredirect.example" };
var didSendMagicLink = await supabase.Auth.SendMagicLink("joseph@supabase.io", options);
```

--------------------------------

### Invoke Supabase Edge Function with basic options (C#)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/functions-invoke

This example demonstrates a basic invocation of a Supabase Edge Function in C#. It shows how to pass custom headers, such as an Authorization token, and a JSON body using `InvokeFunctionOptions`.

```C#
var options = new InvokeFunctionOptions
{
    Headers = new Dictionary<string, string> {{ "Authorization", "Bearer 1234" }},
    Body = new Dictionary<string, object> { { "foo", "bar" } }
};

await supabase.Functions.Invoke("hello", options: options);
```

--------------------------------

### Supabase C# Auth: Sign In User

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithpassword

Demonstrates how to authenticate an existing user with Supabase using either an email and password or a phone number and password. Both methods return a session object upon successful authentication. This functionality requires either an email and password or a phone number and password.

```C#
var session = await supabase.Auth.SignIn(email, password);
```

```C#
var session = await supabase.Auth.SignIn(SignInType.Phone, phoneNumber, password);
```

--------------------------------

### Sign in with OAuth Provider

Source: https://supabase.com/docs/reference/csharp/introduction/index

Signs a user in using a third-party OAuth provider. Ensure you have configured the desired provider in your Supabase project settings.

```csharp
var signInUrl = supabase.Auth.SignIn(Provider.Github);
```

--------------------------------

### Send SMS OTP for Sign-In

Source: https://supabase.com/docs/reference/csharp/introduction/index

Sends a One-Time Password (OTP) via SMS to a user's phone number for passwordless authentication.

```csharp
var options = new SignInOptions { RedirectTo = "http://myredirect.example" };
var didSendSmsOtp = await supabase.Auth.SendOtp("1234567890", options);
```

--------------------------------

### Create Signed URL in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-createsignedurl

Creates a time-limited signed URL for a specific file in a Supabase Storage bucket. This URL allows temporary access to the file without requiring direct user permissions, valid for a specified number of seconds.

```C#
var url = await supabase.Storage.From("avatars").CreateSignedUrl("public/fancy-avatar.png", 60);
```

--------------------------------

### Sign in with SMS OTP and Verify OTP using C# Supabase Auth

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithotp

Illustrates the process of initiating a sign-in via SMS OTP to a phone number and subsequently verifying the OTP to obtain a user session. This involves two distinct calls to the Supabase Auth client.

```C#
await supabase.Auth.SignIn(SignInType.Phone, "+13334445555");

// Paired with `VerifyOTP` to get a session
var session = await supabase.Auth.VerifyOTP("+13334445555", TOKEN, MobileOtpType.SMS);
```

--------------------------------

### Listen to Authentication Events

Source: https://supabase.com/docs/reference/csharp/introduction/index

Subscribes to authentication state changes in Supabase. This allows you to react to events such as user sign-in, sign-out, profile updates, password recovery, and token refreshes.

```csharp
supabase.Auth.AddStateChangedListener((sender, changed) => {
    switch (changed) {
        case AuthState.SignedIn:
            break;
        case AuthState.SignedOut:
            break;
        case AuthState.UserUpdated:
            break;
        case AuthState.PasswordRecovery:
            break;
        case AuthState.TokenRefreshed:
            break;
    }
});
```

--------------------------------

### Retrieve Public URL for File

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves the public URL for an asset in a public bucket. The bucket must be marked as public. Requires no specific object permissions.

```csharp
var publicUrl = supabase.Storage.From("avatars").GetPublicUrl("public/fancy-avatar.png");
```

--------------------------------

### Update the password for an authenticated user

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-updateuser

Changes the password for the currently authenticated user. This operation requires the user to be signed in.

```C#
var attrs = new UserAttributes { Password = "***********" };
var response = await supabase.Auth.Update(attrs);

```

--------------------------------

### Create Signed URL for File

Source: https://supabase.com/docs/reference/csharp/introduction/index

Creates a signed URL to download a file without requiring permissions. The URL is valid for a specified number of seconds. Requires 'select' permission on 'objects'.

```csharp
var url = await supabase.Storage.From("avatars").CreateSignedUrl("public/fancy-avatar.png", 60);
```

--------------------------------

### Invoke Supabase Edge Function

Source: https://supabase.com/docs/reference/csharp/introduction/index

Invokes a Supabase Edge Function. Requires an Authorization header. The invocation parameters generally follow the Fetch API specification, allowing for custom headers and request bodies.

```csharp
var options = new InvokeFunctionOptions
{
    Headers = new Dictionary<string, string> { { "Authorization", "Bearer 1234" } },
    Body = new Dictionary<string, object> { { "foo", "bar" } }
};
await supabase.Functions.Invoke("hello", options: options);
```

--------------------------------

### Sign In User with Email and Password

Source: https://supabase.com/docs/reference/csharp/introduction/index

Logs in an existing user using their email address and password. This is a standard authentication method.

```csharp
var session = await supabase.Auth.SignIn(email, password);
```

--------------------------------

### Authentication Operations in C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Enables user management functionalities such as signing up, signing in with password or OTP, signing out, and managing user sessions. It also allows listening to authentication state changes.

```csharp
await supabase.Auth.SignUp("test@example.com", "password");
supabase.Auth.OnAuthStateChange((authState, user) => { /* Handle auth state changes */ });
await supabase.Auth.SignInWithPassword("test@example.com", "password");
await supabase.Auth.SignInWithOtp(new AuthOtpRequest { Email = "test@example.com" });
await supabase.Auth.SignInWithOAuth(new OAuthRequest { Provider = OAuthProvider.Google });
await supabase.Auth.SignOut();
await supabase.Auth.VerifyOtp(new VerifyOtpRequest { Email = "test@example.com", Token = "123456" });
await supabase.Auth.GetSession();
await supabase.Auth.GetUser();
await supabase.Auth.UpdateUser(new User { Email = "new@example.com" });
```

--------------------------------

### Sign In User with Phone and Password

Source: https://supabase.com/docs/reference/csharp/introduction/index

Logs in an existing user using their phone number and password. This is an alternative to email-based sign-in.

```csharp
var session = await supabase.Auth.SignIn(phone, password);
```

--------------------------------

### Verify Sms One-Time Password (OTP)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-verifyotp

The `VerifyOtp` method takes in different verification types. If a phone number is used, the type can either be `sms` or `phone_change`. If an email address is used, the type can be one of the following: `signup`, `magiclink`, `recovery`, `invite` or `email_change`. The verification type used should be determined based on the corresponding auth method called before `VerifyOtp` to sign up / sign-in a user.

```C#
var session = await supabase.Auth.VerifyOTP("+13334445555", TOKEN, MobileOtpType.SMS);
```

--------------------------------

### Verify OTP for Authentication

Source: https://supabase.com/docs/reference/csharp/introduction/index

Verifies a One-Time Password (OTP) sent via SMS or email. The `MobileOtpType` parameter specifies the verification context (e.g., SMS, phone change). For email, the type can be 'signup', 'magiclink', 'recovery', 'invite', or 'email_change'.

```csharp
var session = await supabase.Auth.VerifyOTP("+13334445555", TOKEN, MobileOtpType.SMS);
```

--------------------------------

### Get the current session data in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-getsession

Retrieves the current active session data from the Supabase authentication client. This property returns the session object if a user is logged in, otherwise it will be null.

```C#
var session = supabase.Auth.CurrentSession;
```

--------------------------------

### Invoke Supabase Edge Function with a typed response (C#)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/functions-invoke

This example illustrates how to invoke a Supabase Edge Function and automatically deserialize the response into a C# class. It defines a `HelloResponse` class with a `JsonProperty` attribute for mapping the JSON response.

```C#
class HelloResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }
}

await supabase.Functions.Invoke<HelloResponse>("hello");
```

--------------------------------

### Retrieve public URL for a Supabase Storage asset in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-getpublicurl

This example demonstrates how to retrieve the public URL for a file stored in a Supabase Storage bucket using the C# client library. For this to work, the bucket must be explicitly set to public, either programmatically via `UpdateBucket()` or through the Supabase dashboard. No specific `buckets` or `objects` policy permissions are required for this operation on public assets.

```C#
var publicUrl = supabase.Storage.From("avatars").GetPublicUrl("public/fancy-avatar.png");
```

--------------------------------

### Get the logged in user in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-getuser

Retrieves the currently logged-in user's data from Supabase authentication. This method returns the user object if a user is authenticated.

```C#
var user = supabase.Auth.CurrentUser;
```

--------------------------------

### Retrieve Current User

Source: https://supabase.com/docs/reference/csharp/introduction/index

Fetches the data for the currently logged-in user. This requires a user to be authenticated.

```csharp
var user = supabase.Auth.CurrentUser;
```

--------------------------------

### Sign up a new user with Supabase Auth in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signup

This snippet demonstrates how to sign up a new user using the Supabase C# client's Auth module. It awaits the `SignUp` method with the user's email and password, returning a session object.

```C#
var session = await supabase.Auth.SignUp(email, password);
```

--------------------------------

### Edge Functions Invocation in C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Enables invoking Supabase Edge Functions directly from your C# application.

```csharp
await supabase.Functions.Invoke("my-function", new Dictionary<string, string> { { "key", "value" } });
```

--------------------------------

### Initialize Supabase C# Client with Dependency Injection (Maui-like)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/initializing

This snippet illustrates how to integrate the Supabase C# client using dependency injection, specifically in a Maui-like application context. It registers the client as a singleton service, configuring options such as `AutoRefreshToken` and `AutoConnectRealtime`.

```C#
public static MauiApp CreateMauiApp()
{
      // ...
      var builder = MauiApp.CreateBuilder();

      var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
      var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
      var options = new SupabaseOptions
      {
        AutoRefreshToken = true,
        AutoConnectRealtime = true,
        // SessionHandler = new SupabaseSessionHandler() <-- This must be implemented by the developer
      };

      // Note the creation as a singleton.
      builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));
}
```

--------------------------------

### Update the email for an authenticated user

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-updateuser

Updates the email address for the currently authenticated user. This operation requires the user to be signed in. By default, a confirmation link is sent to both the current and new email addresses, unless 'Secure email change' is disabled in project settings.

```C#
var attrs = new UserAttributes { Email = "new-email@example.com" };
var response = await supabase.Auth.Update(attrs);

```

--------------------------------

### Retrieve Storage Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves the details of an existing Storage bucket. Requires 'select' permission on 'buckets'.

```csharp
var bucket = await supabase.Storage.GetBucket("avatars");
```

--------------------------------

### Sign Out User

Source: https://supabase.com/docs/reference/csharp/introduction/index

Signs out the currently authenticated user. This action requires the user to be signed in prior to execution.

```csharp
await supabase.Auth.SignOut();
```

--------------------------------

### C#: Querying JSON data

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

Shows how to query and filter data within JSON columns using path notation.

```csharp
 var result = await supabase
  .From<Users>()
  .Select("id, name, address->street")
  .Filter("address->postcode", Operator.Equals, 90210)
  .Get();
```

--------------------------------

### Update the user's metadata

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-updateuser

Updates the custom metadata associated with the authenticated user's profile. This operation requires the user to be signed in.

```C#
var attrs = new UserAttributes
{
  Data = new Dictionary<string, string> { {"example", "data" } }
};
var response = await supabase.Auth.Update(attrs);

```

--------------------------------

### Upload File to Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Uploads a file to an existing bucket. Requires 'insert' permission on 'objects'.

```csharp
var imagePath = Path.Combine("Assets", "fancy-avatar.png");
await supabase.Storage.From("avatars").Upload(imagePath, "fancy-avatar.png", new FileOptions { CacheControl = "3600", Upsert = false });
```

--------------------------------

### Retrieve Current Session

Source: https://supabase.com/docs/reference/csharp/introduction/index

Fetches the data for the currently active user session. This requires an active session to be present.

```csharp
var session = supabase.Auth.CurrentSession;
```

--------------------------------

### Sign out a user in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signout

This snippet demonstrates how to sign out the current user. In order to use the `SignOut()` method, the user needs to be signed in first.

```C#
await supabase.Auth.SignOut();
```

--------------------------------

### Storage File Management in C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Offers functionalities for managing files within storage buckets, such as uploading, downloading, listing, replacing, moving, and deleting files. It also supports creating signed URLs and retrieving public URLs.

```csharp
await supabase.Storage.From("my-bucket").Upload(new FileManager("path/to/file.txt", "text/plain", "content"));
await supabase.Storage.From("my-bucket").Download("path/to/file.txt");
await supabase.Storage.From("my-bucket").List();
await supabase.Storage.From("my-bucket").Update("path/to/file.txt", new File "path/to/newfile.txt");
await supabase.Storage.From("my-bucket").Move("path/to/oldfile.txt", "path/to/newlocation/file.txt");
await supabase.Storage.From("my-bucket").Remove(new List<string> { "path/to/file.txt" });
await supabase.Storage.From("my-bucket").CreateSignedUrl("path/to/file.txt", 60);
await supabase.Storage.From("my-bucket").GetPublicUrl("path/to/file.txt");
```

--------------------------------

### Update Supabase Storage Bucket in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-updatebucket

This C# code demonstrates updating a Supabase Storage bucket named 'avatars' to be private (`Public = false`). The operation requires `buckets` update permissions.

```C#
var bucket = await supabase.Storage.UpdateBucket("avatars", new BucketUpsertOptions { Public = false });
```

--------------------------------

### Download File from Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Downloads a file from a bucket. Requires 'select' permission on 'objects'.

```csharp
var bytes = await supabase.Storage.From("avatars").Download("public/fancy-avatar.png");
```

--------------------------------

### Initialize Supabase Client

Source: https://supabase.com/docs/reference/csharp/introduction/index

Initializes the Supabase client with your project URL and public key. It demonstrates setting up real-time connections and initializing the client asynchronously. Models deriving from `BaseModel` are required for API interaction.

```csharp
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
var options = new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
};
var supabase = new Supabase.Client(url, key, options);
await supabase.InitializeAsync();
```

--------------------------------

### Retrieve a specific Storage bucket in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-getbucket

This example demonstrates how to retrieve the details of an existing Storage bucket using the Supabase C# client. It requires `select` permissions on `buckets`.

```C#
var bucket = await supabase.Storage.GetBucket("avatars");

```

--------------------------------

### Create a Supabase Storage Bucket in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-createbucket

This example demonstrates how to create a new storage bucket named 'avatars' using the Supabase C# client. To successfully create a bucket, the user requires 'insert' permissions on the 'buckets' table. No specific permissions are needed for 'objects'.

```C#
var bucket = await supabase.Storage.CreateBucket("avatars");
```

--------------------------------

### List All Storage Buckets

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves the details of all Storage buckets. Requires 'select' permission on 'buckets'.

```csharp
var buckets = await supabase.Storage.ListBuckets();
```

--------------------------------

### Match records in C# using Supabase client

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/match

Demonstrates how to find database records using the Supabase C# client's `Match` method. Examples include matching with a C# model object and with a Dictionary<string, string> for column-value pairs. This is useful for hydrating models and correlating with database entries.

```csharp
var city = new City
{
    Id = 224,
    Name = "Atlanta"
};

var model = supabase.From<City>().Match(city).Single();
```

```csharp
var opts = new Dictionary<string, string>
{
    {"name","Beijing"},
    {"country_id", "156"}
};

var model = supabase.From<City>().Match(opts).Single();
```

--------------------------------

### Applying Basic Filters in C# Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/using-filters

This snippet demonstrates how to apply a basic equality filter to a column using a LINQ-like `Where` clause on a `Select()` query. It retrieves a single record where the 'Name' property matches 'The Shire'.

```C#
var result = await supabase.From<City>()
      .Select(x => new object[] { x.Name, x.CountryId })
      .Where(x => x.Name == "The Shire")
      .Single();
```

--------------------------------

### Limit Number of Rows Returned

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves a limited number of rows from the 'City' table, specifically the first 10. It selects 'Name' and 'CountryId' columns.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Limit(10).Get();
```

--------------------------------

### Initialize Supabase C# Client Standardly

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/initializing

This example demonstrates the standard way to initialize the Supabase C# client by providing the project URL and API key. It sets up basic options like `AutoConnectRealtime` and initializes the client asynchronously.

```C#
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

var options = new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
};

var supabase = new Supabase.Client(url, key, options);
await supabase.InitializeAsync();
```

--------------------------------

### Limit Query to a Range

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves rows from the 'City' table within a specified range (inclusive), from index 0 to 3. It selects 'name' and 'country_id' columns using raw SQL.

```csharp
var result = await supabase.From<City>().Select("name, country_id").Range(0, 3).Get();
```

--------------------------------

### C#: Getting your data from Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

Demonstrates how to fetch all data from a Supabase table using a C# model and the `Get()` method.

```csharp
// Given the following Model (City.cs)
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }

    //... etc.
}

// A result can be fetched like so.
var result = await supabase.From<City>().Get();
var cities = result.Models
```

--------------------------------

### C#: Querying with count option

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

Demonstrates how to retrieve the count of rows matching a query with different count types using the `Count()` method.

```csharp
var count = await supabase
  .From<Movie>()
  .Select(x => new object[] { x.Name })
  .Count(CountType.Exact);
```

--------------------------------

### Listen to presence sync in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Explains how to track user presence using Supabase Realtime channels in C#. It involves registering a custom class for presence data, adding an event handler for sync events, subscribing, and tracking presence updates.

```C#
  class UserPresence : BasePresence
  {
      [JsonProperty("cursorX")]
      public bool IsTyping {get; set;}

      [JsonProperty("onlineAt")]
      public DateTime OnlineAt {get; set;}
  }

  var channel = supabase.Realtime.Channel("any");
  var presenceKey = Guid.NewGuid().ToString();
  var presence = channel.Register<UserPresence>(presenceKey);
  presence.AddPresenceEventHandler(EventType.Sync, (sender, type) =>
  {
      Debug.WriteLine($"The Event Type: {type}");
      var state = presence.CurrentState;
  });

  await channel.Subscribe();

  // Send a presence update
  await presence.Track(new UserPresence { IsTyping = false, OnlineAt = DateTime.Now });

```

--------------------------------

### Replace File in Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Replaces an existing file at the specified path with a new one. Requires 'update' and 'select' permissions on 'objects'.

```csharp
var imagePath = Path.Combine("Assets", "fancy-avatar.png");
await supabase.Storage.From("avatars").Update(imagePath, "fancy-avatar.png");
```

--------------------------------

### Fetch Data with Supabase C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates fetching data from a 'cities' table using the Supabase C# client. It shows how to select specific columns, query foreign tables, filter data, and handle potential limitations like the default row limit and reserved keywords.

```csharp
// Given the following Model (City.cs)
[Table("cities")]
public class City : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("country_id")]
    public int CountryId { get; set; }
    //... etc.
}

// A result can be fetched like so.
var result = await supabase.From<City>().Get();
var cities = result.Models;
```

--------------------------------

### Perform websearch full-text search in C# with Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

This example demonstrates how to perform a websearch-style full-text search (WFTS) in C#. It uses `Operator.WFTS` to find matches, which is suitable for web-style search queries on the `Catchphrase` column.

```C#
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.WFTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

--------------------------------

### Upload a file to Supabase Storage in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-upload

Demonstrates how to upload a local file to a specified bucket in Supabase Storage using C#. Examples include basic upload with file options and tracking upload progress with a callback. Requires `insert` permissions on objects.

```C#
var imagePath = Path.Combine("Assets", "fancy-avatar.png");

await supabase.Storage
  .From("avatars")
  .Upload(imagePath, "fancy-avatar.png", new FileOptions { CacheControl = "3600", Upsert = false });
```

```C#
var imagePath = Path.Combine("Assets", "fancy-avatar.png");

await supabase.Storage
  .From("avatars")
  .Upload(imagePath, "fancy-avatar.png", onProgress: (sender, progress) => Debug.WriteLine($"{progress}%"));
```

--------------------------------

### Update file

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-update

Demonstrates how to replace an existing file in a Supabase Storage bucket using the C# client. It specifies the path to the new file and the target file name within the bucket, requiring 'update' and 'select' object permissions.

```C#
var imagePath = Path.Combine("Assets", "fancy-avatar.png");
await supabase.Storage.From("avatars").Update(imagePath, "fancy-avatar.png");

```

--------------------------------

### Filter Rows by Greater Than or Equal To

Source: https://supabase.com/docs/reference/csharp/introduction/index

Shows how to select rows where a column's numeric value is greater than or equal to a specified value. This includes the boundary value in the results.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Where(x => x.CountryId >= 250).Get();
```

--------------------------------

### Retrieve a Single Row

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves exactly one row from the 'City' table. This method will result in an error if the query returns zero or more than one row.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Single();
```

--------------------------------

### C#: Querying foreign tables

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

Illustrates querying data across foreign tables using string-based column selection to retrieve related data.

```csharp
var data = await supabase
  .From<Transactions>()
  .Select("id, supplier:supplier_id(name), purchaser:purchaser_id(name)")
  .Get();
```

--------------------------------

### Filter rows with less than condition using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/lt

This example demonstrates how to retrieve data from a Supabase table and filter rows where a specific column's value is less than a given number. It uses the `Select()` method to specify which columns to return, and the `Where()` clause with a lambda expression for the less than comparison. This requires the Supabase C# client library.

```C#
var result = await supabase.From<City>()
  .Select("name, country_id")
  .Where(x => x.CountryId < 250)
  .Get();
```

--------------------------------

### Filter Rows by Less Than or Equal To

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates selecting rows where a column's numeric value is less than or equal to a specified value. This includes the boundary value in the results.

```csharp
var result = await supabase.From<City>().Where(x => x.CountryId <= 250).Get();
```

--------------------------------

### Create New User

Source: https://supabase.com/docs/reference/csharp/introduction/index

Creates a new user in Supabase. By default, the user must verify their email. The return value depends on the 'Confirm email' setting in the Supabase project. If enabled, only a user object is returned; if disabled, both user and session are returned.

```csharp
var session = await supabase.Auth.SignUp(email, password);
```

--------------------------------

### Filter Rows by Pattern Match (Case-Sensitive)

Source: https://supabase.com/docs/reference/csharp/introduction/index

Shows how to select rows where a string column matches a given pattern using case-sensitive matching. The '%' wildcard can be used for partial matches.

```csharp
var result = await supabase.From<City>().Filter(x => x.Name, Operator.Like, "%la%").Get();
```

--------------------------------

### Filter Rows by Inequality

Source: https://supabase.com/docs/reference/csharp/introduction/index

Illustrates how to select rows where a column's value does not match a specified value. This is useful for excluding certain data from query results.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Where(x => x.Name != "Bali").Get();
```

--------------------------------

### Perform basic normalized text search in C# with Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

This example shows how to perform a basic normalized full-text search (PLFTS) in C#. It uses `Operator.PLFTS` to find matches after applying basic normalization to the tsvector value of the `Catchphrase` column.

```C#
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.PLFTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

--------------------------------

### List all Storage buckets in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-listbuckets

This example demonstrates how to retrieve a list of all Storage buckets using the Supabase C# client. It requires `select` permissions on `buckets` and no permissions on `objects`.

```C#
var buckets = await supabase.Storage.ListBuckets();

```

--------------------------------

### Update User Attributes

Source: https://supabase.com/docs/reference/csharp/introduction/index

Updates user data, such as email or metadata. By default, email updates send a confirmation to both old and new emails. This behavior can be modified in Supabase project settings.

```csharp
var attrs = new newUserAttributes { Email = "new-email@example.com" };
var response = await supabase.Auth.Update(attrs);
```

--------------------------------

### Update Storage Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Updates an existing Storage bucket. Requires 'update' permission on 'buckets'.

```csharp
var bucket = await supabase.Storage.UpdateBucket("avatars", new BucketUpsertOptions { Public = false });
```

--------------------------------

### Filter data with OR condition using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/or

Demonstrates how to use the `Where` clause with an OR condition (`||`) to select rows based on multiple criteria, followed by a `Get()` call to retrieve the results from a Supabase table.

```csharp
var result = await supabase.From<Country>()
  .Where(x => x.Id == 20 || x.Id == 30)
  .Get();
```

--------------------------------

### Realtime Subscriptions in C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Allows subscribing to and unsubscribing from channels to listen for real-time database changes. It also provides functionality to retrieve all active channels.

```csharp
var channel = supabase.Subscribe("*", (message) => { /* Handle messages */ });
await supabase.RemoveChannel(channel);
supabase.GetChannels();
```

--------------------------------

### Filter by Values within a JSON Column in C# Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/using-filters

This example shows how to filter records based on a value nested within a JSON column. It uses the `Filter` method with a path notation ('address->postcode') and an `Operator.Equals` to match a specific postcode.

```C#
var result = await supabase.From<City>()
  .Filter("address->postcode", Operator.Equals, 90210)
  .Get();
```

--------------------------------

### Fetch inserted records after an insert operation (C#)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/insert

This example demonstrates how to retrieve the inserted record(s) immediately after an insert operation. It uses `QueryOptions { Returning = ReturnType.Representation }` to get the representation of the inserted data back from the Supabase API.

```C#
var result = await supabase
  .From<City>()
  .Insert(models, new QueryOptions { Returning = ReturnType.Representation });

```

--------------------------------

### Create Storage Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Creates a new Storage bucket in Supabase. Requires 'insert' permission on 'buckets'.

```csharp
var bucket = await supabase.Storage.CreateBucket("avatars");
```

--------------------------------

### Move file in C# Supabase Storage

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-move

Demonstrates how to move a file from one path to another within a Supabase Storage bucket using the C# client. This operation can also be used to rename a file. It requires 'update' and 'select' object permissions.

```C#
await supabase.Storage.From("avatars")
  .Move("public/fancy-avatar.png", "private/fancy-avatar.png");

```

--------------------------------

### List Files in Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Lists all files within a bucket. Requires 'select' permission on 'objects'.

```csharp
var objects = await supabase.Storage.From("avatars").List();
```

--------------------------------

### Filter rows by exact column value using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/is

This C# code snippet demonstrates how to use the Supabase client to filter rows from a table (`City`) where a specific column (`Name`) has an exact value of `null`. It uses the `Where` clause with a lambda expression and retrieves the results using `Get()`.

```csharp
var result = await supabase.From<City>()
  .Where(x => x.Name == null)
  .Get();
```

--------------------------------

### Filter Rows by Pattern Match (Case-Insensitive)

Source: https://supabase.com/docs/reference/csharp/introduction/index

Illustrates selecting rows where a string column matches a pattern using case-insensitive matching. This is useful for flexible text searches.

```csharp
await supabase.From<City>().Filter(x => x.Name, Operator.ILike, "%la%").Get();
```

--------------------------------

### Filter data using ContainedIn operator with Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/containedby

This C# code snippet demonstrates how to filter data from a Supabase table using the `ContainedIn` operator. It retrieves `City` records where the `MainExports` property contains any of the specified values ('oil', 'fish'). The `Get()` method executes the query asynchronously.

```C#
var result = await supabase.From<City>()
  .Filter(x => x.MainExports, Operator.ContainedIn, new List<object> { "oil", "fish" })
  .Get();

```

--------------------------------

### C#: Selecting specific columns from a table

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

Shows how to select only specific columns from a Supabase table using LINQ expressions with the `Select()` method.

```csharp
// Given the following Model (Movie.cs)
[Table("movies")]
class Movie : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    //... etc.
}

// A result can be fetched like so.
var result = await supabase
  .From<Movie>()
  .Select(x => new object[] {x.Name, x.CreatedAt})
  .Get();
```

--------------------------------

### Storage Bucket Management in C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Provides methods for managing storage buckets, including creating, retrieving, listing, updating, and deleting buckets, as well as emptying a bucket.

```csharp
await supabase.Storage.CreateBucket("my-bucket");
await supabase.Storage.GetBucket("my-bucket");
await supabase.Storage.ListBuckets();
await supabase.Storage.UpdateBucket("my-bucket", new UpdateBucketRequest { Name = "my-new-bucket-name" });
await supabase.Storage.DeleteBucket("my-bucket");
await supabase.Storage.EmptyBucket("my-bucket");
```

--------------------------------

### C#: Filter data where column is not equal to a value using Select()

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/neq

This C# example demonstrates how to query data from a Supabase table (`City`), select specific columns (`Name`, `CountryId`), and apply a filter to retrieve only rows where the `Name` column's value is not equal to 'Bali'. It uses the `Select()` and `Where()` methods of the Supabase client.

```C#
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.Name != "Bali")
  .Get();
```

--------------------------------

### Filter data with case-insensitive pattern in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/ilike

Demonstrates how to filter rows in a Supabase table where a specific column's value matches a case-insensitive pattern using `Operator.ILike`.

```C#
await supabase.From<City>()
  .Filter(x => x.Name, Operator.ILike, "%la%")
  .Get();
```

--------------------------------

### Call a Postgres stored procedure without parameters in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/rpc

Demonstrates how to invoke a Postgres stored procedure (RPC) from C# using the Supabase client without passing any parameters. The `Rpc` method is used with the function name and `null` for parameters.

```C#
await supabase.Rpc("hello_world", null);
```

--------------------------------

### Listen to inserts on a specific table in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Illustrates how to specifically listen for `INSERT` events on a table (`City`) in a Supabase database using the C# client library.

```C#
await supabase.From<City>().On(ListenType.Inserts, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

--------------------------------

### Perform basic text search in C# with Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

This example demonstrates how to perform a basic full-text search (FTS) on a specified column using the `Operator.FTS` in C#. It finds rows where the `Catchphrase` column's tsvector value matches the provided `to_tsquery` string, configured for English.

```C#
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.FTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

--------------------------------

### Realtime Channel Subscriptions

Source: https://supabase.com/docs/reference/csharp/introduction/index

Manages real-time subscriptions to database changes. Supports listening to broadcast messages, presence sync, specific tables, all database changes, or specific event types (inserts, updates, deletes, row level changes). Realtime must be enabled in project settings.

```csharp
public class CursorBroadcast : BaseBroadcast
{
    [JsonProperty("cursorX")]
    public int CursorX { get; set; }
    [JsonProperty("cursorY")]
    public int CursorY { get; set; }
}

var channel = supabase.Realtime.Channel("any");
var broadcast = channel.Register<CursorBroadcast>();
broadcast.AddBroadcastEventHandler((sender, baseBroadcast) =>
{
    var response = broadcast.Current();
});
await channel.Subscribe();

// Send a broadcast
await broadcast.Send("cursor", new CursorBroadcast { CursorX = 123, CursorY = 456 });
```

--------------------------------

### Perform full normalized text search in C# with Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

This example illustrates how to perform a full normalized full-text search (PHFTS) in C#. It utilizes `Operator.PHFTS` to find matches after applying comprehensive normalization to the tsvector value of the `Catchphrase` column.

```C#
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.PHFTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

--------------------------------

### Database Operations in C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Provides methods for interacting with your Postgres database, including fetching, inserting, updating, and deleting data. It also supports calling Postgres functions.

```csharp
await supabase.From<User>().Select("*").Get();
await supabase.From<User>().Insert(new User { Name = "Test" });
await supabase.From<User>().Update(new User { Name = "Test Updated" }).Where(x => x.Id == "1");
await supabase.From<User>().Delete().Where(x => x.Id == "1");
await supabase.Rpc("get_user_by_email", new Dictionary<string, string> { { "email", "test@example.com" } });
```

--------------------------------

### Filter rows where column is greater than a value using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/gt

Demonstrates how to query data from a Supabase table in C# using the `Select()` method to specify returned columns and the `Where()` method with a greater than (>) condition to filter rows based on a column's value. It retrieves cities where `CountryId` is greater than 250.

```C#
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.CountryId > 250)
  .Get();
```

--------------------------------

### Filter rows that don't match a value using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/not

This C# code snippet demonstrates how to retrieve data from a 'Country' table, selecting specific columns (Name, CountryId), and filtering out rows where the 'Name' column is 'Paris'. It uses the Supabase client's `From`, `Select`, `Where`, and `Get` methods.

```C#
var result = await supabase.From<Country>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.Name != "Paris")
  .Get();
```

--------------------------------

### Filter Rows by Null or Specific Value

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates checking for exact equality with a specific value, including null. This is used for finding rows that precisely match a given condition.

```csharp
var result = await supabase.From<City>().Where(x => x.Name == null).Get();
```

--------------------------------

### Upsert Data with Supabase C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Illustrates how to perform an upsert operation (insert or update) on the 'cities' table using the Supabase C# client. It emphasizes the requirement for primary keys in the data payload for updates to function correctly.

```csharp
var model = new City { Id = 554, Name = "Middle Earth" };
await supabase.From<City>().Upsert(model);
```

--------------------------------

### List files in a bucket

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-list

This example demonstrates how to list all files within a specified Supabase storage bucket using the C# client. This operation requires `select` permissions on objects.

```C#
var objects = await supabase.Storage.From("avatars").List();

```

--------------------------------

### Filter rows where column is less than or equal to a value using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/lte

This C# code snippet demonstrates how to query a Supabase table to retrieve rows where a specific column's value is less than or equal to a given integer. It uses the `Where()` method with a lambda expression for filtering and `Get()` to execute the query and fetch results.

```C#
var result = await supabase.From<City>()
  .Where(x => x.CountryId <= 250)
  .Get();
```

--------------------------------

### Interact with Supabase C# Client using Models

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/initializing

This example demonstrates how to define and use a C# model (`Message`) that maps to a Supabase database table. It shows common CRUD operations (Get, Insert, Update, Delete) using the strongly-typed `BaseModel` and `Table` attributes for mapping.

```C#
// Given the following Model representing the Supabase Database (Message.cs)
[Table("messages")]
public class Message : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("username")]
    public string UserName { get; set; }

    [Column("channel_id")]
    public int ChannelId { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Message message &&
                Id == message.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}

void Initialize()
{
    // Get All Messages
    var response = await client.Table<Message>().Get();
    List<Message> models = response.Models;

    // Insert
    var newMessage = new Message { UserName = "acupofjose", ChannelId = 1 };
    await client.Table<Message>().Insert();

    // Update
    var model = response.Models.First();
    model.UserName = "elrhomariyounes";
    await model.Update();

    // Delete
    await response.Models.Last().Delete();

    // etc.
}
```

--------------------------------

### Select Rows Without Filter

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves rows from the database that do not satisfy a specified filter condition. It selects specific columns ('Name' and 'CountryId') from the 'Country' table.

```csharp
var result = await supabase.From<Country>().Select(x => new object[] { x.Name, x.CountryId }).Where(x => x.Name != "Paris").Get();
```

--------------------------------

### Full-Text Search (FTS)

Source: https://supabase.com/docs/reference/csharp/introduction/index

Shows how to perform full-text searches on a `tsvector` column using Supabase's filtering capabilities. Supports basic, websearch, and normalized text search configurations.

```csharp
var result = await supabase.From<Quote>().Select(x => x.Catchphrase).Filter(x => x.Catchphrase, Operator.FTS, new FullTextSearchConfig("'fat' & 'cat", "english")).Get();
```

--------------------------------

### Supabase C# Client Library Dependency Updates

Source: https://supabase.com/docs/reference/csharp/introduction/index

Details the updates to various Supabase C# client library dependencies, including changes to namespaces, package names, and specific method updates. This includes Postgrest, GoTrue, Realtime, Storage, Functions, and Core components.

```csharp
Assembly Name has been changed to `Supabase.dll`
Update dependency: `postgrest-csharp@5.0.0`
  * [MAJOR] Moves namespaces from `Postgrest` to `Supabase.Postgrest`
Update dependency: `gotrue-csharp@5.0.0`
  * Re: [#89](https://github.com/supabase-community/gotrue-csharp/issues/89) Update signature for `SignInWithIdToken` which adds an optional `accessToken` parameter, update doc comments, and call `DestroySession` in method
  * Re: [#88](https://github.com/supabase-community/gotrue-csharp/issues/88), Add `IsAnonymous` property to `User`
  * Re: [#90](https://github.com/supabase-community/gotrue-csharp/issues/90) Implement `LinkIdentity` and `UnlinkIdentity`
Update dependency: `realtime-csharp@7.0.0`
  * Merges [#45](https://github.com/supabase-community/realtime-csharp/pull/45) - Updating the `Websocket.Client@5.1.1`
Update dependency: `storage-csharp@2.0.0`
Update dependency: `functions-csharp@2.0.0`
Update dependency: `core-csharp@1.0.0`
```

--------------------------------

### Retrieve All Realtime Channels

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves a list of all currently active Realtime channel subscriptions managed by the client.

```csharp
var channels = supabase.Realtime.Subscriptions;
```

--------------------------------

### Listen to deletes on a specific table in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Illustrates how to specifically listen for `DELETE` events on a table (`City`) in a Supabase database using the C# client library.

```C#
await supabase.From<City>().On(ListenType.Deletes, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

--------------------------------

### Filter Rows by Less Than

Source: https://supabase.com/docs/reference/csharp/introduction/index

Illustrates selecting rows where a column's numeric value is strictly less than a specified value. This is useful for excluding values above a certain threshold.

```csharp
var result = await supabase.From<City>().Select("name, country_id").Where(x => x.CountryId < 250).Get();
```

--------------------------------

### Empty bucket

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-emptybucket

Demonstrates how to remove all objects from a specified Supabase Storage bucket using the C# client library. This operation requires 'select' permissions on 'buckets' and 'select' and 'delete' permissions on 'objects'.

```C#
var bucket = await supabase.Storage.EmptyBucket("avatars");
```

--------------------------------

### Filter rows where column is greater than or equal to a value using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/gte

This example demonstrates how to query data from a Supabase table (`City`) to find rows where the `CountryId` column is greater than or equal to a specific value (250). It also shows how to use the `Select()` method to project only the `Name` and `CountryId` columns from the results.

```csharp
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.CountryId >= 250)
  .Get();
```

--------------------------------

### Empty Storage Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Removes all objects inside a single bucket. Requires 'select' and 'delete' permissions on 'objects'.

```csharp
var bucket = await supabase.Storage.EmptyBucket("avatars");
```

--------------------------------

### Listen to row-level changes in a table in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Explains how to subscribe to realtime changes for specific rows within a table (`countries`) based on a filter condition (`id=eq.200`) using Supabase Realtime channels in C#.

```C#
var channel = supabase.Realtime.Channel("realtime", "public", "countries", "id", "id=eq.200");

channel.AddPostgresChangeHandler(ListenType.All, (sender, change) =>
{
    // The event type
    Debug.WriteLine(change.Event);
    // The changed record
    Debug.WriteLine(change.Payload);
});

await channel.Subscribe();

```

--------------------------------

### Filter C# data with Supabase using `Like` operator

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/like

This C# code snippet demonstrates how to filter rows in a Supabase table where a specific column's value matches a supplied pattern using the `Like` operator. It performs a case-sensitive search for the pattern within the column, returning the matching results.

```C#
var result = await supabase.From<City>()
  .Filter(x => x.Name, Operator.Like, "%la%")
  .Get();
```

--------------------------------

### Call a Postgres stored procedure with parameters in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/rpc

Illustrates how to call a Postgres stored procedure (RPC) from C# using the Supabase client, passing a dictionary of parameters. The `Rpc` method takes the function name and a `Dictionary<string, object>` containing the parameters.

```C#
await supabase.Rpc("hello_world", new Dictionary<string, object> { { "foo", "bar"} });
```

--------------------------------

### Download a file from Supabase Storage in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-download

This snippet demonstrates how to download a file from a Supabase Storage bucket using the C# client library. It includes examples for a basic download and a download with progress tracking.

```C#
var bytes = await supabase.Storage.From("avatars").Download("public/fancy-avatar.png");
```

```C#
var bytes = await supabase.Storage
  .From("avatars")
  .Download("public/fancy-avatar.png", (sender, progress) => Debug.WriteLine($"{progress}%"));
```

--------------------------------

### Move File in Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Moves an existing file within a bucket, optionally renaming it. Requires 'update' and 'select' permissions on 'objects'.

```csharp
await supabase.Storage.From("avatars").Move("public/fancy-avatar.png", "private/fancy-avatar.png");
```

--------------------------------

### Retrieve all Realtime channels in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/getchannels

This code snippet demonstrates how to access all currently active Realtime channel subscriptions from the Supabase client instance. The `Subscriptions` property returns a collection of all joined channels.

```C#
var channels = supabase.Realtime.Subscriptions;
```

--------------------------------

### Filter Rows by Value within an Array

Source: https://supabase.com/docs/reference/csharp/introduction/index

Shows how to select rows where a column's value is present in a provided list of values. This is equivalent to an SQL 'IN' clause.

```csharp
var result = await supabase.From<City>().Filter(x => x.Name, Operator.In, new List<object> { "Rio de Janiero", "San Francisco" }).Get();
```

--------------------------------

### Insert Data with Supabase C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Provides an example of inserting a new record into the 'cities' table using the Supabase C# client. It includes the model definition with table and column attributes.

```csharp
[Table("cities")]
public class City : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("country_id")]
    public int CountryId { get; set; }
}

var model = new City { Name = "The Shire", CountryId = 554 };
await supabase.From<City>().Insert(model);
```

--------------------------------

### Listen to updates on a specific table in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Illustrates how to specifically listen for `UPDATE` events on a table (`City`) in a Supabase database using the C# client library.

```C#
await supabase.From<City>().On(ListenType.Updates, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

--------------------------------

### Combine OR and AND conditions for data filtering in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/or

Illustrates how to combine OR conditions (`||`) within one `Where` clause and chain it with another `Where` clause for an implicit AND condition, retrieving rows that satisfy both sets of criteria from a Supabase table.

```csharp
var result = await supabase.From<Country>()
  .Where(x => x.Population > 300000 || x.BirthRate < 0.6)
  .Where(x => x.Name != "Mordor")
  .Get();
```

--------------------------------

### Filter Rows by Equality

Source: https://supabase.com/docs/reference/csharp/introduction/index

Shows how to select rows where a specific column's value exactly matches a given value. This is a fundamental filtering operation for retrieving precise data.

```csharp
var result = await supabase.From<City>().Where(x => x.Name == "Bali").Get();
```

--------------------------------

### Update Data with Supabase C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates how to update existing data in the 'cities' table using the Supabase C# client. It shows updating a record based on a filter condition.

```csharp
var update = await supabase.From<City>().Where(x => x.Name == "Auckland").Set(x => x.Name, "Middle Earth").Update();
```

--------------------------------

### Delete Files in Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Deletes files within the same bucket. Requires 'delete' and 'select' permissions on 'objects'.

```csharp
await supabase.Storage.From("avatars").Remove(new List<string> { "public/fancy-avatar.png" });
```

--------------------------------

### Limit rows with embedded resources in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/limit

This example demonstrates limiting rows within an embedded resource (e.g., a nested relationship) using the `Limit()` method in a Supabase query.

```C#
var result = await supabase.From<Country>()
  .Select("name, cities(name)")
  .Filter("name", Operator.Equals, "United States")
  .Limit(10, "cities")
  .Get();
```

--------------------------------

### Install Supabase C# Client via NuGet

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/installing

Use this command in your project directory to add the Supabase C# client library as a NuGet package, making its functionalities available for use.

```C#
dotnet add package supabase
```

--------------------------------

### Delete Storage Bucket

Source: https://supabase.com/docs/reference/csharp/introduction/index

Deletes an existing Storage bucket. The bucket must be empty. Requires 'select' and 'delete' permissions on 'buckets'.

```csharp
var result = await supabase.Storage.DeleteBucket("avatars");
```

--------------------------------

### C#: Filtering with inner joins

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

Explains how to filter data using inner joins with related tables and specific conditions.

```csharp
var result = await supabase
  .From<Movie>()
  .Select("*, users!inner(*)")
  .Filter("user.username", Operator.Equals, "Jane")
  .Get();
```

--------------------------------

### Select Rows Matching At Least One Filter

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves rows from the database that satisfy at least one of the specified filter conditions using the OR operator. It selects specific columns ('Name' and 'CountryId') from the 'Country' table.

```csharp
var result = await supabase.From<Country>().Where(x => x.Id == 20 || x.Id == 30).Get();
```

--------------------------------

### Listen to all database changes in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Shows how to subscribe to all realtime changes across an entire schema (`public`) or all tables (`*`) in a Supabase database using the C# client library. It includes handling different event types and payload data.

```C#
var channel = supabase.Realtime.Channel("realtime", "public", "*");

channel.AddPostgresChangeHandler(ListenType.All, (sender, change) =>
{
    // The event type
    Debug.WriteLine(change.Event);
    // The changed record
    Debug.WriteLine(change.Payload);
});

await channel.Subscribe();

```

--------------------------------

### Match Model with Dictionary

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates how to find a model instance by matching it against a dictionary of key-value pairs. This is useful for hydrating models and correlating data.

```csharp
var city = new City { Id = 224, Name = "Atlanta" };
var model = supabase.From<City>().Match(city).Single();
```

--------------------------------

### Filter Rows by Greater Than

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates selecting rows where a column's numeric value is strictly greater than a specified value. This is commonly used for range-based queries.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Where(x => x.CountryId > 250).Get();
```

--------------------------------

### Call a Postgres Function (RPC)

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates how to call a stored procedure in PostgreSQL using the Supabase client's Remote Procedure Call (RPC) functionality. This is useful for executing database logic from the application.

```csharp
await supabase.Rpc("hello_world", null);
```

--------------------------------

### Bulk insert multiple records into a Supabase table (C#)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/insert

This example shows how to perform a bulk insert of multiple new records into a Supabase table. It uses a `List<City>` to insert several `City` instances in a single operation via `supabase.From<City>().Insert(models)`.

```C#
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }
}

var models = new List<City>
{
  new City { Name = "The Shire", CountryId = 554 },
  new City { Name = "Rohan", CountryId = 553 },
};

await supabase.From<City>().Insert(models);

```

--------------------------------

### Upsert data with conflict resolution in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/upsert

Illustrates how to handle conflicts during an UPSERT operation by specifying a column (e.g., 'Name') to resolve conflicts. If a record with the same 'Name' exists, it will be updated; otherwise, a new record will be inserted.

```C#
var model = new City
{
  Id = 554,
  Name = "Middle Earth"
};

await supabase
  .From<City>()
  .OnConflict(x => x.Name)
  .Upsert(model);
```

--------------------------------

### Filter rows where column equals value with Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/eq

Demonstrates how to filter rows in a Supabase table where a specific column's value exactly matches a given string, using the 'Where' clause and 'Get' method with 'Select()' in C#.

```C#
var result = await supabase.From<City>()
  .Where(x => x.Name == "Bali")
  .Get();
```

--------------------------------

### Limit rows with Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/limit

This example demonstrates how to limit the number of rows returned when using a `Select()` projection in a Supabase query.

```C#
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Limit(10)
  .Get();
```

--------------------------------

### Retrieve a single row with Select()

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/single

This example demonstrates how to retrieve a single row of data from a Supabase table using the `Select()` method to specify columns and then applying the `Single()` method to ensure only one row is returned. It will throw an error if more than one row matches the query.

```C#
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Single();
```

--------------------------------

### Listen to specific table changes in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Demonstrates how to listen to all types of changes (inserts, updates, deletes) for a specific table (`City`) in a Supabase database using the C# client library.

```C#
await supabase.From<City>().On(ListenType.All, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

--------------------------------

### Delete Data with Supabase C#

Source: https://supabase.com/docs/reference/csharp/introduction/index

Shows how to delete records from the 'cities' table using the Supabase C# client. It highlights the importance of using filters to specify which records to delete.

```csharp
await supabase.From<City>().Where(x => x.Id == 342).Delete();
```

--------------------------------

### Delete a file from Supabase Storage in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-remove

This example demonstrates how to delete one or more files from a specified Supabase storage bucket using the C# client library. It requires `delete` and `select` permissions on objects within the bucket.

```C#
await supabase.Storage.From("avatars").Remove(new List<string> { "public/fancy-avatar.png" });
```

--------------------------------

### Filter Rows by Array Contained Within Value

Source: https://supabase.com/docs/reference/csharp/introduction/index

Demonstrates selecting rows where a column (typically an array type) is entirely contained within a specified list of values. This checks if the column's array is a subset of the provided list.

```csharp
var result = await supabase.From<City>().Filter(x => x.MainExports, Operator.ContainedIn, new List<object> { "oil", "fish" }).Get();
```

--------------------------------

### Filter data where a column contains all elements using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/contains

This C# code snippet demonstrates how to filter a table (`City`) where a specific column (`MainExports`) contains all elements from a provided list (`"oil", "fish"`). It uses the `Filter` method with the `Operator.Contains` to perform this check, retrieving the results asynchronously.

```C#
var result = await supabase.From<City>()
  .Filter(x => x.MainExports, Operator.Contains, new List<object> { "oil", "fish" })
  .Get();
```

--------------------------------

### Order Results by Column

Source: https://supabase.com/docs/reference/csharp/introduction/index

Retrieves rows from the 'City' table, ordering them by 'Id' in descending order. It selects 'Name' and 'CountryId' columns.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Order(x => x.Id, Ordering.Descending).Get();
```

--------------------------------

### Listen to broadcast messages in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

Demonstrates how to listen to and send broadcast messages using Supabase Realtime channels in C#. It involves registering a custom class for broadcast data, adding an event handler, subscribing to the channel, and sending messages.

```C#
class CursorBroadcast : BaseBroadcast
{
    [JsonProperty("cursorX")]
    public int CursorX {get; set;}

    [JsonProperty("cursorY")]
    public int CursorY {get; set;}
}

var channel = supabase.Realtime.Channel("any");
var broadcast = channel.Register<CursorBroadcast>();
broadcast.AddBroadcastEventHandler((sender, baseBroadcast) =>
{
    var response = broadcast.Current();
});

await channel.Subscribe();

// Send a broadcast
await broadcast.Send("cursor", new CursorBroadcast { CursorX = 123, CursorY = 456 });

```

--------------------------------

### Upsert data in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/upsert

Demonstrates a basic UPSERT operation into a Supabase table using a C# model. The primary key (Id) is included in the model for the upsert to correctly identify and update or insert the record.

```C#
var model = new City
{
  Id = 554,
  Name = "Middle Earth"
};

await supabase.From<City>().Upsert(model);
```

--------------------------------

### Limit query results with Range() and Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/range

This example demonstrates how to limit the number of rows returned by a Supabase query using the `Range()` method in C#. It selects specific columns ('name', 'country_id') from the 'City' table and retrieves rows from index 0 to 3 (inclusive).

```C#
var result = await supabase.From<City>()
  .Select("name, country_id")
  .Range(0, 3)
  .Get();
```

--------------------------------

### Update Data with Filter and Set in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/update

Demonstrates how to update data in a Supabase table by filtering records using a 'Where' clause and directly setting new values with the 'Set' method. This approach performs an update operation on matching records without first retrieving them.

```C#
var update = await supabase
  .From<City>()
  .Where(x => x.Name == "Auckland")
  .Set(x => x.Name, "Middle Earth")
  .Update();
```

--------------------------------

### Remove a Realtime channel in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/removechannel

Demonstrates how to unsubscribe and remove a Realtime channel from the Supabase Realtime client in C# using two different approaches: unsubscribing from a channel obtained via `From<City>()` or directly from a named channel.

```C#
var channel = await supabase.From<City>().On(ChannelEventType.All, (sender, change) => { });
channel.Unsubscribe();

// OR

var channel = supabase.Realtime.Channel("realtime", "public", "*");
channel.Unsubscribe()
```

--------------------------------

### Filter Rows by Array Containing Elements

Source: https://supabase.com/docs/reference/csharp/introduction/index

Illustrates selecting rows where a column (typically an array type) contains all the elements specified in a given list. This is useful for checking subset relationships.

```csharp
var result = await supabase.From<City>().Filter(x => x.MainExports, Operator.Contains, new List<object> { "oil", "fish" }).Get();
```

--------------------------------

### Filter rows where column value is in an array using Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/in

This C# code snippet demonstrates how to use the `Filter` method with the `Operator.In` to retrieve rows from a 'City' table where the 'Name' column matches any of the values provided in a list. It fetches all columns implicitly via `Get()` after filtering.

```C#
var result = await supabase.From<City>()
  .Filter(x => x.Name, Operator.In, new List<object> { "Rio de Janiero", "San Francisco" })
  .Get();
```

--------------------------------

### Insert a single record into a Supabase table (C#)

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/insert

This example demonstrates how to insert a single new record into a Supabase table. It defines a `City` model with `Id`, `Name`, and `CountryId` properties and then inserts an instance of this model using `supabase.From<City>().Insert(model)`.

```C#
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }
}

var model = new City
{
  Name = "The Shire",
  CountryId = 554
};

await supabase.From<City>().Insert(model);

```

--------------------------------

### Install Supabase C# Client

Source: https://supabase.com/docs/reference/csharp/introduction/index

Installs the Supabase C# client library using the .NET CLI. This is the first step to integrating Supabase into your C# application.

```bash
dotnet add package supabase
```

--------------------------------

### Upsert data and return exact row count in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/upsert

Shows how to perform an UPSERT operation while requesting an exact count of affected rows. This is achieved by passing a QueryOptions object with Count set to QueryOptions.CountType.Exact to the Upsert method.

```C#
var model = new City
{
  Id = 554,
  Name = "Middle Earth"
};

await supabase
  .From<City>()
  .Upsert(model, new QueryOptions { Count = QueryOptions.CountType.Exact });
```

--------------------------------

### Filter Foreign Tables in C# Supabase

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/using-filters

This snippet illustrates how to filter records from a primary table based on conditions in a related foreign table. It uses a `Select` statement with an inner join syntax and then applies a `Filter` on the foreign table's column ('cities.name') to retrieve countries associated with cities named 'Bali'.

```C#
var results = await supabase.From<Country>()
  .Select("name, cities!inner(name)")
  .Filter("cities.name", Operator.Equals, "Bali")
  .Get();
```

--------------------------------

### Delete Supabase Storage bucket in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-deletebucket

This code snippet demonstrates how to delete an existing Supabase storage bucket using the C# client library. It calls the `DeleteBucket` method on the `supabase.Storage` object, passing the bucket's name.

```C#
var result = await supabase.Storage.DeleteBucket("avatars");
```

--------------------------------

### Update Data by Modifying Retrieved Model in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/update

Illustrates updating data by first retrieving a single record from the Supabase table, modifying its properties, and then calling the 'Update()' method directly on the hydrated model. This method is suitable for updating specific, already loaded entities.

```C#
var model = await supabase
  .From<City>()
  .Where(x => x.Name == "Auckland")
  .Single();

model.Name = "Middle Earth";

await model.Update<City>();
```

--------------------------------

### Select Specific Columns and Filter

Source: https://supabase.com/docs/reference/csharp/introduction/index

Illustrates selecting only specific columns from a table and applying a filter condition. This optimizes queries by reducing data transfer.

```csharp
var result = await supabase.From<City>().Select(x => new object[] { x.Name, x.CountryId }).Where(x => x.Name == "The Shire").Single();
```

--------------------------------

### Order results with embedded resources in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/order

Shows how to order results when selecting embedded resources. This example filters by a parent table column and then orders a nested resource (cities) in descending order.

```C#
var result = await supabase.From<Country>()
  .Select("name, cities(name)")
  .Filter(x => x.Name == "United States")
  .Order("cities", "name", Ordering.Descending)
  .Get();
```

--------------------------------

### Delete specific records from a Supabase table in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/delete

Demonstrates how to delete specific records from a Supabase table in C#. The example uses a `Where` clause to filter records by `Id` before calling the `Delete()` method, ensuring only the targeted data is removed.

```C#
await supabase
  .From<City>()
  .Where(x => x.Id == 342)
  .Delete();
```

--------------------------------

### Unsubscribe from Realtime Channel

Source: https://supabase.com/docs/reference/csharp/introduction/index

Unsubscribes from and removes a Realtime channel from the client. This is important for maintaining performance, especially when listening to many database changes. Supabase handles cleanup, but manual unsubscription is recommended for unused channels.

```csharp
var channel = await supabase.From<City>().On(ChannelEventType.All, (sender, change) => {});
channel.Unsubscribe();

// OR

var channel = supabase.Realtime.Channel("realtime", "public", "*");
channel.Unsubscribe()
```

--------------------------------

### Order parent table by referenced table in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/order

Illustrates ordering a parent table (City) based on a column from a referenced (joined) table (Country). The ordering is applied to the 'name' column of the 'country' alias in ascending order.

```C#
var result = await supabase.From<City>()
  .Select("name, country:countries(name)")
  .Order("country(name)", Ordering.Ascending)
  .Get();
```

--------------------------------

### Order results with Select() in C#

Source: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/order

Demonstrates ordering results using the `Select()` method to specify columns and the `Order()` method for sorting in descending order based on a primary key.

```C#
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Order(x => x.Id, Ordering.Descending)
  .Get();
```