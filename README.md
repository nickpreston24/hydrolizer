# SimVentions Hydrolizer

> Purpose: Convert IDLs (and other syntaxes or languages) into common formats used in DDS. Generate or update full stack
> UI's with DDS ready to go.

### Goals / TodoList (for Nick)

1. Read refactors (Sharpify.Core), or temporary RegexEnumBases...
2. Create 'tests' for each replacement rule

3. To try:
- [ ] Try to make a generator that takes a regex and a given struct and generates an `extraction_map` Func that maps all the values from `Extract()` to it.  Then create an overload for `Extract()` that takes structs. (this should be similar to Vogen's generation - look at their source to see if you can avoid boxing)
- [ ] add a routine & command for adding hydro to an existing project (including imports and the web setup from `Program.cs`)
- [ ] I'd also like a ValueObject (vogen) generation rule for Sharpify, that finds all files with auto properties and asks the dev if they wish to tag refactor them as ValueObjects.
- [ ] Make a cli that takes a file name, finds all variables of interest (namespace, classnames, auto properties, etc) and asks the user to identify which ones they wish to turn into template variables (for hydrations).  The new template files will be saved as `.template.<ext>` and can be used in many applications.
- [ ] Find an existing Hydro* file and replace the namespace with that of the current .csproj name.
- [ ] Create a new, blank Hydro file and name it after the current namespace.
- [ ] Perform a nested substitution where repeats occur - like a long line of LINQ selects.
- [ ] Generate + add new refactors from RegexEnumBase.
- [ ] Try file-based source generators [here](https://thecodeman.net/posts/source-generators-deep-dive) and see if you
  can't read your existing source code and infer things via extract() and make substitutions / hydrations.
    - [ ] Try to generate a Value Object / struct's initialization method (like Vogen's `.From()` method) for a given
      regex that has named groups.
- Add "gmix" back to your sharpify.core.
    - write a test for sharpify.core for "gmix"
- [ ] Create a new RazorHAT hybrid from justdoit, replace namespace and build, removing unused contents.
- [ ] See if you can't target one source project and Extract and ReplaceAll in the same step. I think that I may have
  something here - if we can keep the `$` anchor in our regex substitutions and make substitution, as well as an
  Extract, then why not? It would be pretty great to have a history of what exactly was replaced/subbed and its exact
  position in the line. This can have many powerful applications if done correctly, especially when done with
  `*.refactor.json` files.
    - **it may be best to create an overload of Extract(), which accepts a Grepper class instance and performs a bulk
      extraction, while retaining line numbers and the file text.**

3. After this is done, migrate the code into Sharpify.Core.

## Ideal Control Flow

1. START
   2. Option 1 -  SourceGen
       1. grep for files or templates of interest (hydro,.idls, .json, etc.) internally and generate code.
       2. Vogen is a great example. Here's [another](https://github.com/vaananart/SourceGeneratorDemo-Part2) one that
          generates formulas from math.
   3. Option 2 - Run Sharpify.Core Hydrate methods to perform nested hydrations on existing templates (e.g. a Hydro).
   4. Option 3 - Perform hydrations on existing code, inline.
2. Run generated source code.
3. REPEAT
4. ???
5. $$$