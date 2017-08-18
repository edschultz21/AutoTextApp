
grammar Dql;

dql
 : dql_stmt EOF
 ;

dql_stmt
 : ( get_stmt )?
     from_stmt
   ( where_stmt )?
   ( order_stmt )? 
   ( skiptake_stmt )?
 ;

///////////////////////////////////////////////////////////////
// Top level statements
///////////////////////////////////////////////////////////////

get_stmt
 : GET entity_path ( COMMA entity_path )*
 ;

from_stmt
 : FROM main_table_name ( table_alias )? ( COMMA table_alias_stmt )*
 ;

where_stmt
 : WHERE search_condition
 ;

order_stmt
 : ORDER BY ordering_term ( COMMA ordering_term )*
 ;

skiptake_stmt
 : take_term ( skip_term )?
 | skip_term ( take_term )?
;

///////////////////////////////////////////////////////////////

entity_path
 : segment_name ( DOT segment_name )* ( DOT ASTERISK )?
 ;

table_alias_term
 : table_alias DOT segment_name
 ;

table_alias_stmt
 : table_alias_term ( table_alias_defined )?
 ;

take_term
 : TAKE NUMERAL
 ;

skip_term
 : SKIP_ NUMERAL
 ;

ordering_term
 : entity_path ( ASC | DESC )?
 ;

/*
    DQL operators, in order from highest to lowest precedence:
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
 | expr NOT? IN '(' param_list ')'	# InPred
 | expr NOT? LIKE expr				# LikePred
 | expr IS null_notnull				# IsPred
 | '(' search_condition ')'			# ParensPred
 ;

param_list
 : expr (',' expr)*
 ;

comparison_operator
 : '=' | '>' | '<' | '<=' | '>=' | '!=' | '<>'
 ;

null_notnull
 : NOT? NULL
 ;

expr
 : NULL								# NullExpr
 | constant							# ConstantExpr
 | function_call					# FunctionCallExpr
 | table_alias_term					# TableAliasTermExpr
 | '(' expr ')'						# ParensExpr
 | expr op=('*' | '/' | '%') expr	# MulDivExpr
 | op=('+' | '-') expr				# UnaryExpr
 | expr op=('+' | '-' ) expr		# AddSubExpr
 ;

function_call
 : func_proc_name '(' param_list? ')'
 ;

func_proc_name
 : sql_func_name | custom_func_name
 ;

constant
 : STRING_LITERAL
 | sign? NUMERAL	
 | sign? (REAL | FLOAT)
 ;

sign
 : '+'
 | '-'
 ;

///////////////////////////////////////////////////////////////
// Naming
///////////////////////////////////////////////////////////////

keyword
 : AND
 | ASC
 | BETWEEN
 | BY
 | DESC
 | FROM
 | GET
 | IN
 | IS
 | ISNULL
 | LIKE
 | NOT
 | NOTNULL
 | NULL
 | OR
 | ORDER
 | SKIP_
 | TAKE
 | WHERE
 ;

sql_func_name
 : COUNT 
 | LEFT 
 | RIGHT 
 | LOCATE 
 | CURRENTTIMESTAMP 
 | SECOND 
 | MINUTE 
 | HOUR 
 | DAY 
 | MONTH 
 | YEAR 
 | DATE 
 | CONCAT 
 | UPPER 
 | UCASE 
 | LCASE 
 | LOWER 
 | LENGTH 
 | LTRIM 
 | RTRIM
 | TRIM 
 | REPLACE 
 | SUBSTRING 
 ;

custom_func_name
 : POWERTESTFUNC 
 | GETDATE
 ;

name :					any_name;
segment_name :			any_name;
table_alias :			any_name;
main_table_name :		any_name;
table_alias_defined :	any_name;

any_name
 : IDENTIFIER 
 | keyword
 ;

///////////////////////////////////////////////////////////////
// Definitions
///////////////////////////////////////////////////////////////

// This section MUST come BEFORE spaces.
AND : A N D;
ASC : A S C;
BETWEEN : B E T W E E N;
BY : B Y;
DESC : D E S C;
FROM : F R O M;
GET : G E T;
IN : I N;
IS : I S;
ISNULL : I S N U L L;
LIKE : L I K E;
NOT : N O T;
NOTNULL : N O T N U L L;
NULL : N U L L;
OR : O R;
ORDER : O R D E R;
SKIP_ : S K I P;
TAKE : T A K E;
WHERE : W H E R E;

// Common SQL functions.
COUNT : C O U N T;
LEFT : L E F T;
RIGHT : R I G H T;
LOCATE : L O C A T E; // charindex
CURRENTTIMESTAMP : C U R R E N T T I M E S T A M P; // getdate
SECOND : S E C O N D;
MINUTE : M I N U T E;
HOUR : H O U R;
DAY : D A Y;
MONTH : M O N T H;
YEAR : Y E A R;
DATE : D A T E; // dateadd
CONCAT : C O N C A T;
UPPER : U P P E R;
UCASE : U C A S E;
LCASE : L C A S E;
LOWER : L O W E R;
LENGTH : L E N G T H; // len
LTRIM : L T R I M;
RTRIM : R T R I M;
TRIM : T R I M;
REPLACE : R E P L A C E;
SUBSTRING : S U B S T R I N G;

// Custom functions
POWERTESTFUNC : P O W E R T E S T F U N C;
GETDATE : G E T D A T E;

DOT : '.';
COMMA : ',';
ASTERISK : '*';

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