; Comments
(comment) @comment

; Literals
(string_literal) @string
(character_literal) @string
(integer_literal) @number
(floating_point_literal) @number
(boolean_literal) @boolean
(nothing_literal) @constant.builtin
(date_literal) @constant

; Names and declarations
(namespace_definition
  name: (name) @namespace)

(imports_statement
  alias: (identifier) @namespace)

(class_definition
  name: (identifier) @type)

(module_definition
  name: (identifier) @type)

(structure_definition
  name: (identifier) @type)

(interface_definition
  name: (identifier) @type)

(enum_definition
  name: (identifier) @type)

(delegate_definition
  (function_signature
    name: (identifier) @function))

(function_signature
  name: (identifier) @function.method)

(sub_signature
  name: (identifier) @function.method)

(constructor_definition
  "New" @constructor)

(property_signature
  name: (identifier) @property)

(event_definition
  name: (identifier) @property)

(enum_member
  name: (identifier) @constant)

(type_parameter
  name: (identifier) @type.parameter)

(type_name
  (name) @type)

(variable_declarator
  name: (identifier) @variable)

(parameter
  (variable_declarator
    name: (identifier) @variable.parameter))

(argument
  name: (identifier) @variable.parameter)

; Calls and member access
(invocation_expression
  function: (identifier) @function.call)

(invocation_expression
  function: (member_access_expression
    member: (identifier) @function.method.call))

(member_access_expression
  member: (identifier) @property)

; Keywords
[
  "And"
  "AndAlso"
  "As"
  "Assembly"
  "Async"
  "ByRef"
  "ByVal"
  "Case"
  "Catch"
  "Class"
  "Const"
  "Continue"
  "Delegate"
  "Dim"
  "Do"
  "Each"
  "Else"
  "ElseIf"
  "End"
  "Enum"
  "Event"
  "Exit"
  "Finally"
  "For"
  "Function"
  "Get"
  "Handles"
  "If"
  "Implements"
  "Imports"
  "In"
  "Inherits"
  "Interface"
  "Is"
  "IsNot"
  "Iterator"
  "Like"
  "Loop"
  "Mod"
  "Module"
  "MustInherit"
  "MustOverride"
  "Namespace"
  "New"
  "Next"
  "Not"
  "Of"
  "On"
  "Option"
  "Or"
  "OrElse"
  "Overloads"
  "Overridable"
  "Overrides"
  "ParamArray"
  "Partial"
  "Property"
  "Return"
  "Select"
  "Set"
  "Shadows"
  "Shared"
  "Static"
  "Step"
  "Structure"
  "Sub"
  "Then"
  "Throw"
  "To"
  "Try"
  "Until"
  "Using"
  "When"
  "While"
  "With"
  "WriteOnly"
  "Xor"
] @keyword

[
  "Private"
  "Protected"
  "Public"
  "Friend"
  "ReadOnly"
  "Default"
  "Narrowing"
  "NotOverridable"
  "Optional"
  "Overloads"
  "Overridable"
  "Overrides"
  "Partial"
  "Shadows"
  "Shared"
  "Widening"
] @keyword.modifier

[
  "Binary"
  "Compare"
  "Explicit"
  "Infer"
  "Off"
  "On"
  "Strict"
  "Text"
] @keyword.directive

[
  "True"
  "False"
  "Nothing"
  "Null"
] @constant.builtin

; Operators and punctuation
[
  "+"
  "-"
  "*"
  "/"
  "\\"
  "&"
  "="
  "<"
  ">"
  "<="
  ">="
  "<>"
  ":="
  "+="
  "-="
  "*="
  "/="
  "\\="
  "&="
  "^="
] @operator

[
  "."
  ","
  ":"
] @punctuation.delimiter

[
  "("
  ")"
  "{"
  "}"
] @punctuation.bracket
