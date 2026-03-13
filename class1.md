# 🎓 Class 1: Deep Dive into the Code!

Welcome back to class! Today, we’re opening up the files and looking at the **actual code**. Don't worry if it looks like alien text at first—I'll translate everything for you!

---

## 🏗️ 1. The Blueprints (Entities)
Before we build anything, we need to define what things look like. We call these **"Entities"**.

### 📄 File: `AppUser.cs`
This defines what a "User" is in our system.
```csharp
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```
*   **`IdentityUser`**: This is a "starter kit" from Microsoft. It already knows how to handle emails, passwords, and phone numbers. We just added `FullName` and `CreatedAt` to it!

### 📄 File: `Reminder.cs`
This is the blueprint for a "Reminder."
```csharp
public class Reminder
{
    public Guid Id { get; set; }             // A unique ID (like a fingerprint)
    public string Name { get; set; }         // Who are we paying?
    public decimal Amount { get; set; }      // How much?
    public string Frequency { get; set; }    // How often?
    public bool IsPaid { get; set; }         // Yes or No?
}
```

---

## 🗄️ 2. The Bridge (ApplicationDbContext.cs)
This file is the **Bridge** between our C# code and the **PostgreSQL Database**.

```csharp
public DbSet<Reminder> Reminders => Set<Reminder>();
public DbSet<PaymentHistory> PaymentHistories => Set<PaymentHistory>();
```
*   **`DbSet`**: Think of this as a **Table** in Excel. `DbSet<Reminder>` tells the backend: "Hey, there is a table called Reminders, and it should follow the `Reminder` blueprint."

---

## 🚀 3. The Starting Line (Program.cs)
This is the **First file** that runs when the server starts. It sets everything up.

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```
*   **Translation**: "Hey Backend, use **PostgreSQL** (Npgsql) and find the connection address in the settings file!"

---

## 🎮 4. The Brains (Controllers)
These files receive requests from the phone and decide what to do.

### 📄 File: `AuthController.cs` (The Security Guard)
Important code for **Registering** a user:
```csharp
var result = await _userManager.CreateAsync(user, dto.Password);
if (result.Succeeded) {
    return Ok(new AuthResponseDto { Token = _tokenService.CreateToken(...) });
}
```
*   **What's happening?**: 
    1. It takes the email/password from the phone.
    2. It tells the `UserManager`: "Save this person!"
    3. If it works, it gives the phone a **Token** (the VIP Pass we talked about).

### 📄 File: `RemindersController.cs` (The Reminder Manager)
Important code for **Marking as Paid**:
```csharp
reminder.IsPaid = true;
var history = new PaymentHistory { ... }; // Record it in history
_context.PaymentHistories.Add(history);
await _context.SaveChangesAsync();        // Save the change to the database
```
*   **What's happening?**:
    1. It finds the reminder you clicked.
    2. It flips the `IsPaid` switch to `true`.
    3. It creates a "History" record so you can see it later.
    4. It saves everything permanently!

---

## 💡 Pro Tip for Students:
See that **`await`** keyword everywhere? 
It means: *"This might take a second (like talking to a database), so please wait until it's finished before moving to the next line."*

**Next Step?** Try opening these files in your editor and finding these exact lines! 🔍🎓
