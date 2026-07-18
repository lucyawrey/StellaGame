// Includes
INCLUDE debug.ink
INCLUDE day1.ink
INCLUDE external.ink

// Constants
VAR none = "none"
//Variables
VAR day = 1

// Debug
VAR debug_mode = false
~ debug_mode = get_debug_mode(debug_mode) // Set debug mode externally if applicable.
{debug_mode: -> debug_prompt}
-> start

=== start
-> day_1_dream

=== date
Card: Day {day}
Card: {get_date(day)}
->->