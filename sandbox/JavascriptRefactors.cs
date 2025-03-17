using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.Shargs;

public class JavascriptRefactors : QueuedService
{
    private readonly ArgsMap arguments;

    public JavascriptRefactors(ArgsMap arguments)
    {
        this.arguments = arguments;
        bool run_functions_refactors = arguments.HasCommand("refactor")
                                       && arguments.HasCommand("js");
        Console.WriteLine(nameof(run_functions_refactors) + "??\t" + run_functions_refactors);
        if (run_functions_refactors)
            steps.Add(RefactorFunctionsWithoutReturnVariables);
    }

    private static async Task<string> RefactorFunctionsWithoutReturnVariables()
    {
        Console.WriteLine(nameof(RefactorFunctionsWithoutReturnVariables));
        // read
        string cwd = Directory.GetCurrentDirectory();

        string sample_code = new ReadFile()
            .From("functions.js")
            .At(cwd, "sandbox", "js")
            .text;

        Console.WriteLine(sample_code);
        // refactor
        // https://regex101.com/r/aCfrGw/1

        var replacement_map = new Dictionary<string, string>()
        {
            [@"""
function\s+
(?<function_name>\w+)  # function name (duh!)
\((?<params>.*)\)\s+ # greedily detect anything, even arrow functions, as params.
{\s*?
((var|let|const)\s+?(?<return_var>result)\s+?=(?<assignment>.+?))\s*?
return(?<return_logic>\s+(.*\s*)+)
;\s*?} 

###  WARNING: this times out very easily when other functions are present... needs fixing
"""]
                =
                @"function ${function_name}(${params}, fallback = {})\n var ${return_var} = ${return_logic};\n return result != null ? result : fallback;"
        };


        return string.Empty;
    }
}