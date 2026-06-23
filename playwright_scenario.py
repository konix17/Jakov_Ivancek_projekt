import subprocess
import time
import sys
import os
from playwright.sync_api import sync_playwright

def run_playwright_test():
    print("Starting ASP.NET Core Web server...")
    # Start the web server on a custom port 5078
    web_dir = os.path.join(os.path.dirname(__file__), "Hotel-Mgt.Web")
    env = os.environ.copy()
    env["ASPNETCORE_URLS"] = "http://127.0.0.1:5078"
    env["ASPNETCORE_ENVIRONMENT"] = "Development"
    
    server_process = subprocess.Popen(
        ["dotnet", "run", "--project", web_dir, "--no-launch-profile"],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        env=env
    )
    
    # Wait for the server to spin up
    print("Waiting for server to initialize...")
    time.sleep(6)
    
    # Check if process is still running
    if server_process.poll() is not None:
        print("Error: Web server failed to start.")
        stdout, stderr = server_process.communicate()
        print("STDOUT:", stdout.decode())
        print("STDERR:", stderr.decode())
        sys.exit(1)

    print("Web server is running. Launching Playwright browser...")
    success = False
    
    try:
        with sync_playwright() as p:
            # Launch headless chromium
            browser = p.chromium.launch(headless=True)
            page = browser.new_page()
            
            # Step 1: Navigate to registration page
            print("Step 1: Navigating to register page...")
            page.goto("http://127.0.0.1:5078/Account/Register")
            
            # Step 2: Fill out registration details
            print("Step 2: Registering new test user...")
            test_email = f"testuser_{int(time.time())}@hotel.local"
            page.fill("input[name='Email']", test_email)
            page.fill("input[name='Password']", "TestPass123!")
            page.fill("input[name='ConfirmPassword']", "TestPass123!")
            page.fill("input[name='OIB']", "12345678901")
            
            # Step 3: Submit registration
            print("Step 3: Submitting registration form...")
            page.click("button[type='submit']")
            page.wait_for_timeout(2000) # Wait for redirect
            
            # Step 4: Verify logged in (check for Signed in text or logout button)
            print("Step 4: Verifying user is logged in...")
            assert "Signed in as" in page.content() or "Logout" in page.content(), "Not signed in after registration."
            
            # Step 5: Log out of new user and log in as seeded Admin
            print("Step 5: Logging out and logging in as seeded Admin...")
            page.click("button:has-text('Logout')")
            page.wait_for_timeout(1000)
            page.goto("http://127.0.0.1:5078/Account/Login")
            page.fill("input[name='Email']", "admin@hotelmanager.local")
            page.fill("input[name='Password']", "Admin123!")
            page.click("button[type='submit']")
            page.wait_for_timeout(2000)
            
            # Step 6: Navigate to Setup as Admin
            print("Step 6: Navigating to Setup as Admin...")
            page.goto("http://127.0.0.1:5078/Setup")
            
            # Step 7: Promote current user to Admin
            print("Step 7: Promoting new user to Admin role...")
            user_row = page.locator(f"tr:has-text('{test_email}')")
            user_row.locator("select[name='roleName']").select_option("Admin")
            user_row.locator("button:has-text('Save')").click()
            page.wait_for_timeout(2000)
            
            # Step 8: Log out Admin and log back in as promoted user to apply Admin claims
            print("Step 8: Re-logging as the newly promoted Admin user...")
            page.click("button:has-text('Logout')")
            page.wait_for_timeout(1000)
            page.goto("http://127.0.0.1:5078/Account/Login")
            page.fill("input[name='Email']", test_email)
            page.fill("input[name='Password']", "TestPass123!")
            page.click("button[type='submit']")
            page.wait_for_timeout(2000)
            
            # Step 9: Create a new Hotel as Admin
            print("Step 9: Creating new hotel as Admin...")
            page.goto("http://127.0.0.1:5078/hoteli/create")
            page.fill("input[name='Name']", "Playwright Grand Hotel")
            page.fill("input[name='Address']", "Testing Lane 12")
            page.fill("input[name='City']", "Zagreb")
            page.fill("input[name='Rating']", "5")
            page.fill("input[name='PhoneNumber']", "098111222")
            page.click("form button:has-text('Save')")
            page.wait_for_timeout(2000)
            print(f"After form submission - URL: {page.url}")
            print(f"After form submission - Content Snippet: {page.content()[:1000]}")
            
            # Step 10: Verify hotel exists in listing
            print("Step 10: Verifying hotel is successfully listed...")
            page.goto("http://127.0.0.1:5078/hoteli")
            try:
                assert "Playwright Grand Hotel" in page.content(), "Hotel not found in index list."
            except AssertionError as ae:
                print(f"Assertion failed! Current URL: {page.url}")
                print("Page content snippet:")
                print(page.content()[:2000])
                raise ae
            
            print("Playwright UI test scenario completed successfully (10/10 steps passed).")
            browser.close()
            success = True
            
    except Exception as e:
        print("An error occurred during Playwright test execution:", e)
    finally:
        print("Stopping Web server...")
        server_process.terminate()
        server_process.wait()
        
    if not success:
        sys.exit(1)

if __name__ == "__main__":
    run_playwright_test()
