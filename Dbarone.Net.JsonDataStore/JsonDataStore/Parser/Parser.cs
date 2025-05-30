using Dbarone.Net.Parser;

namespace Dbarone.Net.JsonDataStore;

public class Parser
{
    /// <summary>
    /// 
    /// </summary>
    private static string _grammar = @"
/* Lexer rules */

CHAR_A = ""A"";
CHAR_B = ""B"";
CHAR_C = ""C"";
CHAR_D = ""D"";
CHAR_E = ""E"";
CHAR_F = ""F"";
CHAR_G = ""G"";
CHAR_H = ""H"";
CHAR_I = ""I"";
CHAR_J = ""J"";
CHAR_K = ""K"";
CHAR_L = ""L"";
CHAR_M = ""M"";
CHAR_N = ""N"";
CHAR_O = ""O"";
CHAR_P = ""P"";
CHAR_Q = ""Q"";
CHAR_R = ""R"";
CHAR_S = ""S"";
CHAR_T = ""T"";
CHAR_U = ""U"";
CHAR_V = ""V"";
CHAR_W = ""W"";
CHAR_X = ""X"";
CHAR_Y = ""Y"";
CHAR_Z = ""Z"";

DIGIT = ""[0-9]"";
SPACE = ""\s"";
DOUBLE_QUOTE = ""\\\""""; 
PERCENT = ""%"";
AMPERSAND = ""&"";
QUOTE = ""'"";
LEFT_PAREN = ""\("";
RIGHT_PAREN = ""\)"";
ASTERISK = ""\*"";
PLUS_SIGN = ""\+"";
MINUS_SIGN = ""\-"";
COMMA = "","";
PERIOD = ""\."";
SOLIDUS = ""/"";
COLON = "":"";
SEMICOLON = "";"";
LESS_THAN_OPERATOR = ""<"";
GREATER_THAN_OPERATOR = "">"";
QUESTION_MARK = ""\?"";
EQUALS_OPERATOR = ""="";
UNDERSCORE = ""_"";
VERTICAL_BAR = ""\|"";
LEFT_BRACKET = ""\["";
RIGHT_BRACKET = ""\]"";
E_CHARACTER = ""E"";

/* Parser rules */

simple_latin_upper_case_letter = CHAR_A | CHAR_B | CHAR_C | CHAR_D | CHAR_E | CHAR_F | CHAR_G | CHAR_H | CHAR_I | CHAR_J | CHAR_K | CHAR_L | CHAR_M | CHAR_N | CHAR_O | CHAR_P | CHAR_Q | CHAR_R | CHAR_S | CHAR_T | CHAR_U | CHAR_V | CHAR_W | CHAR_X | CHAR_Y | CHAR_Z;
unsigned_integer = DIGITS:DIGIT+;
sign = SIGN:PLUS_SIGN | SIGN:MINUS_SIGN;
signed_integer = sign?, unsigned_integer;
exact_numeric_literal = (unsigned_integer, (period, unsigned_integer)?) | period, unsigned_integer;
mantissa = exact_numeric_literal;
exponent_character = CHAR_E;
exponent = unsigned_integer | signed_integer;
approximate_numeric_literal = mantissa, exponent_character, exponent;

";

    public static Node Parse(string input, string rootProductionRule)
    {
        var parser = new Dbarone.Net.Parser.Parser(_grammar, rootProductionRule);
        var tokens = parser.Tokenise(input);
        var ast = parser.Parse(input);
        return ast;
    }
}