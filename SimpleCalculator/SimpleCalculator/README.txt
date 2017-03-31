The actual grammar is in Calculator.g4.

When you compile the project, the output *.cs files are placed in the obj directory.
They automatically get included in the build which is why you do not see them as
part of the project.

Antlr4 basically produces two types of classes: listeners and visitors. They both
do the same thing except the visitor MUST walk the tree whereas the listeners do not.

All of the fun happens with the visitors as that is where the logic is to convert 
the parsed input to whatever output you want.


Output from application:
Text: (9 * (10 - 4)) + (7 * 4 - (3 * (4 + 1)))

First way to process parsed tree:
Result: 67

Alternate way to process parsed tree:
(9 MUL (10 SUB 4)) ADD (7 MUL 4 SUB (3 MUL (4 ADD 1)))

Parsed expression:
(prog (expr (expr ( (expr (expr 9) * (expr ( (expr (expr 10) - (expr 4)) ))) )) + (expr ( (expr (expr (expr 7) * (expr 4)) - (expr ( (expr (expr 3) * (expr ( (expr (expr 4) + (expr 1)) ))) ))) ))))

