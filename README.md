# milk-tea-cashier

## Setting Up Firebase Key Path in System Environment Settings

Follow these steps to configure your Firebase key securely:

### Step 1: Open System Properties
1. Press `Windows + S` and search for **Edit the system environment settings**.
2. Click the result to open the **System Properties** window.

### Step 2: Open Environment Variables
1. In the **System Properties** window, navigate to the **Advanced** tab.
2. Click on the **Environment Variables** button at the bottom.

### Step 3: Add a New Environment Variable
1. In the **Environment Variables** window:
    - Under **User variables** (for the current user) or **System variables** (for all users), click **New**.
2. Fill in the following details:
   - **Variable Name**: `FIREBASE_JSON_PATH`
   - **Variable Value**: Path to your Firebase key file (e.g., `C:\path\to\firebase-adminsdk.json`).
3. Click **OK** to save.

### Step 4: Verify the Setup
1. Open a new Command Prompt or PowerShell window.
2. Type `echo %FIREBASE_JSON_PATH%` (Windows) or `echo $FIREBASE_JSON_PATH` (Linux/Mac).
3. Ensure the output matches the file path you entered.

---

You are now ready to use the Firebase key securely in your application! If you encounter issues, verify:
- The variable name and value are correct.
- The file path points to a valid Firebase key file.

Happy coding!
