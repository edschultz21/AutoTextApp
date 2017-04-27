
grammar SqlHib;

sql
 : sql_stmt EOF
 ;

sql_stmt
 :   select_stmt 
   ( from_stmt )?
   ( where_stmt )?
   ( order_stmt )? 
   ( rows_returned )?
 ;

select_stmt
 : SELECT result_column ( COMMA result_column )*
 ;

from_stmt
 : FROM ( table_stmt ( COMMA table_stmt )* )
 ;

where_stmt
 : WHERE search_condition
 ;

order_stmt
 : ORDER BY ordering_term ( COMMA ordering_term )*
 ;

table_stmt
 : table_name ( DOT column_name )? ( table_alias )?
 ;

result_column
 : all_columns | table_name ( DOT ( column_name | all_columns ))?
 ;

rows_returned
 : take_term ( skip_term )?
 | skip_term ( take_term )?
;

take_term
 : TAKE NUMERAL
 ;

skip_term
 : SKIP_ NUMERAL
 ;

ordering_term
 : column_term ( ASC | DESC )?
 ;

column_term
 : ( table_name DOT )? column_name
 ;

/*
    SQL operators, in order from highest to lowest precedence:
    *    /    %
    +    -
    <    <=   >    >=
    =    ==   !=   <>   IS   IS NOT   LIKE
    AND
    OR
*/

search_condition
 : search_condition_and (OR search_condition_and)*
 ;

search_condition_and
 : search_condition_not (AND search_condition_not)*
 ;

search_condition_not
 : NOT? predicate
 ;

predicate
 : expr comparison_operator expr	# ComparisonPred
 | expr NOT? BETWEEN expr AND expr	# BetweenPred
 | expr NOT? LIKE expr				# LikePred
 | expr IS null_notnull				# IsPred
 | '(' search_condition ')'			# ParensPred
 ;

comparison_operator
 : '=' | '>' | '<' | '<=' | '>=' | '!=' | '<>'
 ;

null_notnull
 : NOT? NULL
 ;

expr
 : NULL								# NullExp
 | constant							# ConstantExp
 | column_term						# ColumnExp
 | '(' expr ')'						# ParensExp
 | expr op=('*' | '/' | '%') expr	# MulDivExp
 | op=('+' | '-') expr				# UnaryExp
 | expr op=('+' | '-' ) expr		# AddSubExp
 | expr comparison_operator expr	# ComparisonExp
 ;

constant
 : STRING	
 | sign? NUMERAL	
 | sign? (REAL | FLOAT)
 ;

sign
 : '+'
 | '-'
 ;

DOT : '.';
OPEN_PAREN : '(';
CLOSE_PAREN : ')';
COMMA : ',';
ASTERISK : '*';
ADD : '+';
SUB : '-';
DIV : '/';
MOD : '%';
AMP : '&';
LT : '<';
LT_EQ : '<=';
GT : '>';
GT_EQ : '>=';
EQ : '=';
NOT_EQ1 : '!=';
NOT_EQ2 : '<>';

all_columns
 : ASTERISK
 ;

column_alias
 : IDENTIFIER
 ;

keyword
 : AND
 | ASC
 | BETWEEN
 | BY
 | DESC
 | FROM
 | IS
 | ISNULL
 | LIKE
 | NOT
 | NOTNULL
 | NULL
 | OR
 | ORDER
 | SELECT
 | SKIP_
 | TAKE
 | WHERE
 ;

name
 : any_name
 ;

table_name 
 : any_name
 ;

column_name 
 : any_name
 ;

table_alias 
 : any_name
 ;

any_name
 : IDENTIFIER 
 | STRING_LITERAL
 | keyword
 ;


AND : A N D;
ASC : A S C;
BETWEEN : B E T W E E N;
BY : B Y;
DESC : D E S C;
FROM : F R O M;
IS : I S;
ISNULL : I S N U L L;
LIKE : L I K E;
NOT : N O T;
NOTNULL : N O T N U L L;
NULL : N U L L;
OR : O R;
ORDER : O R D E R;
SELECT : S E L E C T;
SKIP_ : S K I P;
TAKE : T A K E;
WHERE : W H E R E;

NUMERAL :	DIGIT+;
FLOAT :     DIG_DOT_DIG;
REAL :      DIG_DOT_DIG (E [+-]? DIGIT+)?;

IDENTIFIER : [a-zA-Z_] [a-zA-Z_0-9]*;
STRING_LITERAL : '\'' ( ~'\'' | '\'\'' )* '\'';
SPACEs : [ \u000B\t\r\n] -> channel(HIDDEN);

fragment DIGIT : [0-9];
fragment DIG_DOT_DIG : (DIGIT+ '.' DIGIT+ | DIGIT+ '.' | '.' DIGIT+);

fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];
