# MediTrack — Step-by-Step User Guide

Simple English walkthrough of every screen and feature.

---

## Step 1 — Splash Screen (App Opens)

When you double-click MediTrack to open it, the first thing you see is the **Splash Screen**.

- It shows the MediTrack logo and the message **"Initializing MediTrack..."**
- A loading bar animates at the bottom
- After **2 seconds** it automatically moves to the Login screen
- You don't need to click anything here — it loads on its own

---

## Step 2 — Login Screen

After the splash screen, you land on the **Login Screen**.

You will see two sections:
- **Left panel** — a welcome message and the app branding
- **Right panel** — the sign-in form

### If you already have an account — Sign In:

1. Type your **Username or Email** in the first box
2. Type your **Password** in the second box
3. Click the **"Sign In"** button
4. If details are correct → you go to the Dashboard
5. If wrong → a red error message appears: *"Invalid credentials."*

### If you are a new user — Create Account:

1. Click **"Create Account"** (a toggle link at the bottom of the form)
2. The form expands to show more fields:
   - **Full Name** — your real name (e.g. "Deshika Thikshani")
   - **Username** — your unique login name (e.g. "deshika")
   - **Email** — a valid email (e.g. "deshika@gmail.com")
   - **Password** — must be at least 6 characters
   - **Confirm Password** — type the same password again
3. Click **"Create Account"** button

#### What gets checked when you create an account:
- All fields must be filled — if any are empty → *"All fields are required."*
- Email must look like a real email (must have @ and a dot after it) → *"Please enter a valid email address."*
- Password must be at least 6 characters → *"Password must be at least 6 characters."*
- Both passwords must match → *"Passwords do not match."*
- Username and email must not already exist → *"Username or email already exists."*

4. If everything is correct → account is created and you go straight to the **Dashboard**

---

## Step 3 — Dashboard (Home Screen)

The **Dashboard** is your home page. It shows you a summary of everything happening today.

### Left Sidebar (always visible on all screens)

On the left there is a dark blue panel with:
- The MediTrack logo at the top
- **6 navigation buttons** with icons:
  - 🏠 Dashboard
  - 💊 My Medicines
  - 📅 Schedule
  - 🔔 Reminders
  - 🕐 History
  - ⚙️ Settings
- A greeting at the bottom ("Good morning, Deshika")
- A **Logout** button

Click any button to switch to that screen.

### Top Bar

- Shows your **greeting** (Good morning / afternoon / evening) and today's date
- Shows the **live clock** (updates every second)
- A **"+ Add Medicine"** button to quickly add a new medication

### Stats Cards (4 boxes across the top)

| Card | What it shows |
|---|---|
| **Total Medicines** | How many active medications you have |
| **Taken Today** | How many doses you've already taken today |
| **Missed This Week** | How many doses you missed in the last 7 days |
| **Weekly Adherence** | Your percentage of doses taken in the last 7 days (e.g. 85%) |

### Today's Medicines Section

This shows the medicines you need to take **today**:
- Each card shows the **medicine name**, **scheduled time**, and a **status badge**
- Status badge colors:
  - 🟠 **Orange = Pending** (time hasn't passed yet — upcoming)
  - ⚠️ **Red = Missed** (time has passed and you didn't take it)
  - 🟢 **Green = Taken** (you already took it)
  - ⬜ **Grey = Skipped**
- **"✓ Taken"** green button — click to mark as taken
- **"✗ Skip"** orange button — click to skip (only for upcoming, not missed)
- If there are more than 3 doses, a **"+X more → View All in My Medicines"** link appears

### Right Column (3 small cards)

**Next Dose Required:**
- Shows what time your next upcoming dose is
- Shows the medicine name
- If no doses left today: shows "--:--" and "No upcoming doses"

**Weekly Adherence Chart:**
- A bar chart showing 7 days (oldest on left, Today on right)
- Bar colors: 🟢 Green = good (≥70%), 🟠 Orange = moderate (40–69%), 🔴 Red = poor (<40%)
- Click the expand icon ⛶ to see a bigger version of the chart

**Recent Activity:**
- Shows the last 5 things you did (took or missed a medicine)
- Green dot = Taken, Red dot = Missed/Dismissed
- Shows the time you did the action

### My Medications Table (bottom section)

- Shows up to 5 of your active medicines
- Each row shows: medicine name, type badge (blue), dosage, frequency, Active/Archived status
- **"Edit"** button — opens the edit popup for that medicine
- **"Delete"** button — removes the medicine (soft delete, doesn't destroy history)
- **"View All →"** and **"View All Medications →"** links go to the My Medicines screen

---

## Step 4 — Adding a Medicine

You can add a medicine from the **Dashboard** (click "+ Add Medicine" in the top bar) or from the **My Medicines** screen (click "+ Add Medicine" button).

Both open the same **Add Medicine popup**.

### How to fill in the Add Medicine form:

#### Basic Information section:
1. **Medicine Name *** — official name (e.g. "Amoxicillin") — *required*
2. **Display Name** — a nickname for your own reference (e.g. "Pink antibiotic") — optional
3. **Medicine Type** — choose from dropdown: Tablet / Capsule / Syrup / Injection / Drops / Inhaler / Other
4. **Dose Amount *** — how much per dose (e.g. 250) — *required, must be > 0*
5. **Dose Unit** — choose: tablet / capsule / ml / drops / puff / mg / g

#### Schedule section:
6. **Frequency** — choose: Once daily / Twice daily / Three times daily / Every X hours / Weekly / Custom
7. **Meal Timing** — choose: Before Meal / After Meal / Any Time
8. **Reminder Times** — type times separated by commas (e.g. "08:00,20:00" for 8 AM and 8 PM)
9. **Start Date** — when you start taking this medicine
10. **End Date** — when you stop (optional if ongoing)
11. **Ongoing medication checkbox** — tick if this is a long-term medicine (no end date)

#### Stock & Instructions section:
12. **Remaining Pills** — how many pills you have left (optional, for stock tracking)
13. **Low Stock Alert At** — alert you when pills drop to this number (optional)
14. **Instructions** — any special notes (e.g. "Take with food")

#### Saving:
- Click **"Save Medicine"** — the medicine is saved
- Click **"Cancel"** — closes without saving

#### Validation rules:
- Medicine name is required
- You cannot add a medicine with the same name as an existing active medicine
- Dose amount must be greater than 0

---

## Step 5 — My Medicines Screen

Click **"My Medicines"** in the sidebar.

### What you see:

- **Search bar** — type any part of the name to filter instantly
- **Filter buttons** — All / Active / Archived
- A **count badge** showing "X medications found"
- A table of all your medicines

### Each medicine row shows:
- Medicine name (with display name below if set)
- Type badge (e.g. "Tablet" in blue)
- Dosage (e.g. "250 mg · Once daily")
- Active/Archived status badge
- **"Edit"** button — open the edit form
- **"Archive/Restore"** button — toggle archive status

### Archive vs Delete:
- **Archive** — hides the medicine from active view but keeps all history. Good for medicines you no longer take.
- **Delete** — soft delete, marks as inactive (history still preserved)
- To see archived medicines: click **"Archived"** filter button

### Editing a medicine:
1. Click "Edit" on any medicine
2. Same form as Add Medicine opens, pre-filled with current values
3. Change what you need
4. Click "Save Medicine"

---

## Step 6 — Schedule Screen

Click **"Schedule"** in the sidebar.

The Schedule screen shows your **day-by-day dose timeline**.

### Date Navigation:
- **"‹"** button — go to previous day
- **"›"** button — go to next day
- **"Today"** button — jump back to today
- The current date shows in the center

### Three info cards at the top:

1. **Progress card** — shows "X / Y medicines taken" and a percentage badge
2. **Next Medication card** — shows the next upcoming dose time and name
3. **Status Legend** — color guide: 🟢 Taken, 🟠 Pending, 🔴 Skipped

### Timeline list:
Each medicine scheduled for the selected day appears as a card:
- Time shown on the left in blue (e.g. "08:00")
- Medicine name and dosage in the center
- Status badge (⏳ Pending / ✓ Taken / ✗ Skipped / ⚠ Missed)
- **"✓ Taken"** button — appears for Pending (upcoming) and Missed doses
- **"✗ Skip"** button — appears only for upcoming (Pending) doses

#### Status color coding on the card:
- 🟠 Orange left stripe = Pending (upcoming)
- 🔴 Red left stripe = Missed (time passed, not taken)
- 🟢 Green left stripe = Taken
- ⬜ Grey left stripe = Skipped

**Tip:** You can still mark a missed dose as Taken — just click the green "Taken" button even if it shows red/missed.

---

## Step 7 — Reminders Screen

Click **"Reminders"** in the sidebar.

This is where you set up **when** to receive reminder popups for each medicine.

### Notifications Toggle Card (at the top):

- A large card showing whether notifications are active
- 🟢 Green = "Notifications Active" — reminders will pop up at scheduled times
- 🟡 Yellow = "Notifications Paused" — no reminders will fire
- Click the **toggle switch** on the right to turn notifications on or off

### Reminder Rules section:

Shows all your configured reminders. Each card shows:
- Medicine name
- Time badge (blue, e.g. "08:00")
- Active days badge (green, e.g. "Mon, Wed, Fri" or "Every day")
- Frequency badge (orange, e.g. "Twice daily")
- Snooze badge (purple, e.g. "Snooze 10min")
- ✏️ Edit button
- 🗑️ Delete button

### Adding a new reminder:

Click **"+ Add Reminder"** in the top right.

A popup opens with these fields:

1. **Medicine** — choose which medicine from dropdown *required*

2. **Reminder Times** — add one or more times:
   - Choose Hour (1–12), Minute (00–59), AM/PM
   - Click **"+ Add Time"** to add more times (for twice/three times daily)
   - Click ✕ to remove a time

3. **Snooze Duration** — how many minutes to snooze if needed (5, 10, 15, 30, or 60)

4. **Active Days** — 7 toggle buttons (Mon Tue Wed Thu Fri Sat Sun)
   - Click a day to select/deselect it (selected = dark blue, deselected = light grey)
   - Example: click Mon, Wed, Fri for every other weekday
   - At least one day must be selected

5. **Start Date** — when this reminder starts

6. **End Date** — when it ends (disabled if "Ongoing" is ticked)

7. **"Ongoing medication — no end date"** checkbox — tick for long-term medicines

8. **"Enabled"** checkbox — whether this reminder is currently active

Click **"Save Reminder"** to save, or **"Cancel"** to close.

---

## Step 8 — The Reminder Popup (Alert Window)

When it's time for a medicine, a **small popup window** appears in the **bottom-right corner** of your screen.

- It stays on top of all other windows
- It plays a sound when it appears

### The popup shows:
- Title: "Medication Reminder"
- Medicine name (in yellow)
- Dosage (e.g. "250 mg")

### Three buttons:
- ✅ **"Taken"** (green) — you took the medicine → logged as Taken, popup closes
- ❌ **"Dismiss"** (orange) — you're skipping → logged as Missed, popup closes
- 💤 **"Snooze"** — remind you again after the snooze duration you set (e.g. 10 minutes)

---

## Step 9 — History Screen

Click **"History"** in the sidebar.

Shows every dose action you've ever recorded.

### Filter tabs at the top:
- **All** (dark blue) — shows everything
- **Taken** (green) — only doses you took
- **Skipped** (red) — only doses you dismissed/missed
- **Pending** (orange) — still pending

The count at the top shows "X records found" for the current filter.

### Each record card shows:
- Colored left stripe (green = Taken, red = Skipped/Missed, orange = Pending)
- Medicine name
- Scheduled date and time
- Status badge
- Action time (when you actually clicked Taken or Skipped)

---

## Step 10 — Settings Screen

Click **"Settings"** in the sidebar.

### Account Card:

Shows your profile:
- Colored circle with your first initial (e.g. "D" for Deshika)
- Full name and email address

**To edit your profile:**
1. Click **"Edit Profile"** button
2. Change your Full Name or Email
3. Click **"Save Changes"**
4. If email is already used by someone else → error message shown

**To change your password:**
1. Click **"Change Password"** button
2. Enter your **current password**
3. Enter a **new password** (at least 6 characters)
4. Enter the new password again in **Confirm New Password**
5. Click **"Save Password"**
6. Rules: new password must be ≥ 6 characters, both new passwords must match, current password must be correct

### Preferences Card:

- **Sound Notifications** toggle — turn on/off the sound that plays when a reminder fires

### About Card:

- Shows the app version number (1.0.0)

---

## Step 11 — Logging Out

**From any screen:**
- Click the **"Logout"** button at the bottom of the left sidebar
- You are taken back to the Login screen
- The reminder engine stops (no more alerts until you log in again)

**From the system tray:**
- If you close the window, the app minimizes to the system tray (bottom-right of taskbar)
- Right-click the tray icon → choose **"Exit"** to fully close the app
- Or choose **"Open MediTrack"** to bring it back

---

## Quick Summary — All Features

| Feature | Where to find it |
|---|---|
| Create account | Login screen → "Create Account" |
| Sign in | Login screen |
| View today's doses | Dashboard |
| Mark a dose as taken | Dashboard or Schedule screen |
| See upcoming dose | Dashboard (Next Dose card) |
| View 7-day chart | Dashboard (right column) |
| Add a new medicine | Dashboard top bar or My Medicines screen |
| Edit a medicine | My Medicines → Edit button |
| Archive a medicine | My Medicines → Archive button |
| View all medicines | My Medicines screen |
| Browse by date | Schedule screen |
| Mark a missed dose | Schedule screen |
| Add a reminder time | Reminders screen → + Add Reminder |
| Set active days (Mon-Sun) | Reminders → Add/Edit Reminder popup |
| Turn off all notifications | Reminders → Notifications toggle |
| Snooze a reminder | Alert popup → Snooze button |
| View full history | History screen |
| Filter history by status | History screen → filter tabs |
| Update profile | Settings → Edit Profile |
| Change password | Settings → Change Password |
| Logout | Sidebar → Logout button |
