grammar Calculator;
 
@parser::members
{
    protected const int EOF = Eof;
}
 
@lexer::members
{
    protected const int EOF = Eof;
    protected const int HIDDEN = Hidden;
}
 
/*
 * Parser Rules
 */
 
prog: expr+ ;
 
expr : expr op=(MUL|DIV) expr   # MulDiv
     | expr op=('+'|'-') expr   # AddSub
     | INT                  # int
     | LPAREN expr RPAREN   # parens
     ;
 
/*
 * Lexer Rules
 */
LPAREN : '(';
RPAREN : ')';
INT : [0-9]+;
MUL : '*';
DIV : '/';
ADD : '+';
SUB : '-';
WS
    :   (' ' | '\r' | '\n') -> channel(HIDDEN)
    ;