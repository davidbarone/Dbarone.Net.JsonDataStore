using Dbarone.Net.Parser;

namespace Dbarone.Net.JsonDataStore;

public class Parser
{
    private static string _grammar = @"
/* Lexer rules */

CREATE = ""\bCREATE\b"";
COLLECTION = ""\bCOLLECTION\b"";
LEFT_PAREN = ""[(]"";
RIGHT_PAREN = ""[)]"";
COMMA = "","";
IDENTIFIER      = ""[A-Z_][A-Z_0-9]*"";
DATA_TYPE_INTEGER = ""INT"";
DATA_TYPE_REAL = ""REAL"";
DATA_TYPE_TEXT = ""TEXT"";
DATA_TYPE_DATETIME = ""DATETIME"";

/* Parser rules */

column_name = COLUMN_NAME:IDENTIFIER;
column_definition = COLUMN_NAMES:column_name, DATA_TYPE:data_type;
collection_element_list = LEFT_PAREN!, COLUMNS:column_definition, RIGHT_PAREN!;
table_definition = CREATE!, COLLECTION!, COLUMNS:collection_element_list;
data_type = :DATA_TYPE_INTEGER | :DATA_TYPE_REAL | :DATA_TYPE_TEXT | :DATA_TYPE_DATETIME;

statement = STATEMENT:table_defintion;

    ";

    public static Node Parse(string input)
    {
        var parser = new Dbarone.Net.Parser.Parser(_grammar, "statement");
        var ast = parser.Parse(input);
        return ast;
    }
}