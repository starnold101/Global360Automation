
using Microsoft.Playwright;
using NUnit.Framework;

namespace Global360Automation.Tests;

public class UnitTest1
{
    [Test]
    public async Task CreateMacBookProAsset()
    {
        // Start Playwright and launch a visible Chromium browser.
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 150
            });

        var page = await browser.NewPageAsync();

        // Step 1: Open the Snipe-IT login page.
        await page.GotoAsync("https://demo.snipeitapp.com/login");

        // Step 2: Log in with the demo administrator account.
        await page.GetByLabel("Username").FillAsync("admin");
        await page.Locator("#password-field").FillAsync("password");

        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Login" }
        ).ClickAsync();

        // Step 3: Open the new asset form.
        await page.GotoAsync("https://demo.snipeitapp.com/hardware/create");

        // Step 4: Read the automatically generated asset tag.
        var assetTag = await page.Locator("#asset_tag").InputValueAsync();

        Console.WriteLine($"Asset tag: {assetTag}");

        // Step 5: Open the model dropdown.
        await page.Locator("#select2-model_select_id-container").ClickAsync();

        // Search for the MacBook Pro 13-inch model.
        var modelSearch = page.Locator(
            ".select2-container--open .select2-search__field");

        await modelSearch.FillAsync("Macbook Pro 13");

        // Wait for matching model options to appear.
        var modelOptions = page.Locator(
            ".select2-container--open .select2-results__option");

        await modelOptions.First.WaitForAsync();

        var modelCount = await modelOptions.CountAsync();

        Console.WriteLine($"Model search returned {modelCount} result(s).");

        // Print the available model options.
        for (var i = 0; i < modelCount; i++)
        {
            var optionText = await modelOptions.Nth(i).InnerTextAsync();
            Console.WriteLine($"Option {i + 1}: {optionText}");
        }

        // Select the matching MacBook Pro model.
        var macbookModel = modelOptions.Filter(
            new LocatorFilterOptions
            {
                HasText = "Macbook Pro 13"
            });

        await macbookModel.First.ClickAsync();

        // Step 6: Set the asset status to Ready to Deploy.
        await page.Locator("#status_select_id").SelectOptionAsync("1");

        // Step 7: Expand the optional information section.
        await page.Locator("#optional_info").ClickAsync();

        await page.Locator("#optional_details").WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });

        // Step 8: Enter the asset name.
        await page.Locator("#name").FillAsync("MacBook Pro 13-inch");

        Console.WriteLine("Asset name: MacBook Pro 13-inch");
        Console.WriteLine("Status: Ready to Deploy");

        // Step 9: Save the asset automatically.
        await page.Locator("#submit_button").ClickAsync();

        // Step 10: Wait until the asset list page loads.
        await page.WaitForURLAsync("**/hardware");

        // Verify that the save operation returned to the asset list.
        Assert.That(page.Url, Does.Contain("/hardware"));

        Console.WriteLine($"Page after save: {page.Url}");
        Console.WriteLine($"Asset tag: {assetTag}");
        Console.WriteLine("Asset creation workflow completed successfully.");
    }
}
