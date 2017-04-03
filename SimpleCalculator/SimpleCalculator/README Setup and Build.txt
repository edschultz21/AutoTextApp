Given that there are quite a few (incorrect and outdated) ways of installing
the required packages, and given that one of the packages has some quirks in
how it works, this is a brief readme to get around these.

1. The main package that does all the work is Antlr4. Note that we MUST have v4
   and not v3 as the grammar (syntax) is quite different. To install, search for
   Antlr4 in NuGet Packages. This should be by Sam Harwell. The version I was 
   using was v4.5.3. This will install Antlr4 as well as Antlr4.Runtime.
2. Now we need a grammar. THE FOLLOWING STEPS ARE INCREDIBLY IMPORTANT.
   a. Create, or copy and modify and existing grammar. 
      (At one point it was a requirement that this be save in code page 65001.
	  I have tested this without and it seems to be working fine in the 
	  latest version.)
   b. The name of the file MUST end with .g4.
   c. The name of the grammar (top line in .g4 file), MUST match the name of the
      file. (eg, file Calculator.g4 would have a grammar Calculator;)
	  This is important as ALL of your visitors, listeners and other generated
	  items will have this prefix associated with them.
3. The files are output to the obj directory. DO NOT add these to your project!
   Visual Studio automatically loads everything in the obj directory into your 
   solution! (Including .cs generated files)
4. Most of the work is done in the generated Visitors. Note that these can whatever
   you need them to do. You can also have multiple Visitors doing different things.
