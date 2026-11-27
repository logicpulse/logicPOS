using System.Text;
using LogicPOS.Application.Features.System.Users;

// Interactive PBKDF2 verifier generator for SoftwareProducer configuration.
// Prints only the verifier string to stdout. Never accepts the password as an argv argument.

if (args.Length > 0)
{
    Console.Error.WriteLine("Usage: run with no arguments. The password is prompted interactively (no echo).");
    return 1;
}

Console.Error.Write("Password: ");
var password = ReadPasswordWithoutEcho();
if (string.IsNullOrEmpty(password))
{
    Console.Error.WriteLine("Password must not be empty.");
    return 1;
}

Console.Error.Write("Confirm password: ");
var confirm = ReadPasswordWithoutEcho();
if (!string.Equals(password, confirm, StringComparison.Ordinal))
{
    Console.Error.WriteLine("Passwords do not match.");
    return 1;
}

var verifier = SoftwareProducerPbkdf2.CreateVerifier(password);
Console.WriteLine(verifier);
return 0;

static string ReadPasswordWithoutEcho()
{
    var buffer = new StringBuilder();
    while (true)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter)
        {
            Console.Error.WriteLine();
            break;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (buffer.Length > 0)
                buffer.Length--;
            continue;
        }

        if (!char.IsControl(key.KeyChar))
            buffer.Append(key.KeyChar);
    }

    return buffer.ToString();
}
