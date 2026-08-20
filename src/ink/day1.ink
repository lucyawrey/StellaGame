=== day_1_dream
~ play_music("relaxing_piano_music")
Dream: You float formless in the space between.
Dream: It is quiet. It is comfortable. It is vast beyond comprehension.
Dream: And it is so, so empty.
Dream: You have been alone out here for so very long.
Dream: Floating in the vast expanse.
Dream: But, just now, you notice a small mote of light before you.
Dream: It feels familiar. It fxeels warm.
Dream: You reach out to it.
Dream: Light.
Dream: Blinding.
Dream: Suddenly, a woman stands before you.
Dream: She holds a gleaming blade aloft.
Dream: The void you have always known fills with color and memory.
// Show glowing light
Dream: And then the blade of light falls.
Dream: The concept of death is new to you.
Dream: Yet, somehow, you know it is coming for you.
// Show aborted slashing animation
// Play light fizzling out sound effect
Stella.Dream: Not like this.
~ auto(1)
Dream: _
~ delay(1)
-> date ->
-> day_1_bedroom

=== day_1_bedroom
~ background("bedroom")
{day_1_bedroom > 1: -> act | -> enter}

= enter
~ play_music("background_noise")
Stella.Narrator: I open my eyes and find myself lying halfway off my bed.
Stella.Narrator: Somehow, all of my blankets are discarded on the floor.
Stella.Narrator: The next thing I notice is that I feel sweaty and a bit sick.
Stella.Narrator: At least the bright morning sun coming through the window helps.
Stella: I do not want to get up.
~ delay(0.5)
~ play_music("typhoon_parade")
Stella.Narrator: My phone's alarm starts blaring mid 2010's J-pop as if in direct rejection of my plea.
Stella: Ughhhh.
Stella.Narrator: I only set it to go off on days I have work. Which means I actually have to get up.
Stella.Narrator: I do not even remember where I'm supposed to be working today.
Stella.Narrator: Is it the cafe or the hospital?
Stella: I could just call in sick.
Stella: What are they going to do, fire me?
Stella.Slow: . . .
Stella: Whoever they are.
Stella.Narrator: The alarm continues to go off.
Stella.Narrator: Shutting it off from here is impossible because my phone is all the way across the room, exactly where I left it.
Stella.Narrator: Specifically to ensure I have to actually get out of bed.
Stella.Narrator: It just keeps going.
Stella.Narrator: I breifly concider blowing it up.
Stella.Narrator: But eventually, I resolve myself to fate.
Stella.Narrator: I pull myself out of bed to turn the cursed thing off.
~ play_sound("click")
~ stop_music()
~ play_music("morning")
~ delay(0.5)
Stella: I could use a shower anyway.
Stella.Narrator: Might help with the nausea.
Stella.Narrator: I take a breath and run my fingers through my hair.
Stella.Narrator: It is damp with sweat and somehow even messier than normal.
Stella.Narrator: I definitely need that shower. And I need to check where I'm supposed to be today.
-> act

= act
~ play_music("morning")
+ {!investigate_bedroom or !sleep_in or !check_calendar or !check_phone} [Act]
    ** (investigate_bedroom) [Investigate bedroom]
        The bed is large has an array of cute plushies set on top of it.
        The ramaining room is well decorated and comfortable, but a massive pile of discarded clothes lay on the floor.
        Stella.Narrator: I think this is everything I own.
        *** (pick_up_clothes) [Pick up clothes]
            ~ add_item("clothes_pile", 1)
            Stella put all of the clothes in her pocket.
        *** [Do not]
            This is where they belong.
            Stella.Narrator: I might put them away properly later.
    ** (sleep_in) {!day_1_bathroom.act.take_shower} [Go back to bed]
        Stella: I am really tired...
        Stella: Maybe I can sleep for just a bit longer.
        // Play music, background noise
        ~ auto(1)
        Stella.Slow: . . .
        Stella.Narrator: If I stay in bed too long I will fall back asleep.
        Stella.Narrator: Without an alarm this time.
        ~ auto(1)
        Stella.Slow: . . .
        ~ play_music("morning")
        ~ background("bedroom")
        Stella.Narrator: I better get up.
    ** (get_dressed) {day_1_bathroom.act.take_shower} [Get dressed]
    ** (check_calendar) [Check calendar]
        The dessert themed wall calendar on the wall has a tasty-yet-incriminating ice cream cone drawn next to the month name: July.
        It has not been changed in three months.
        Stella.Slow: . . .
        Stella.Narrator: I should just check my phone.
    ** (check_phone) {check_calendar} [Check phone calendar]
        Ignoring several unread messages, Stella checks the calendar app on her phone.
        Thankfully, it automatically updates when schedules are sent over.
        Stella: Right, I am working at the cafe today. Like almost every other Monday.
        Stella: {sleep_in: It looks like I am already late. | It looks like I am definately going to be a bit late. }
    ++ [Do nothing]
* (talk) [Talk]
    Stella: There is nobody here but me, right?
+ [Move]
    ++ [Bathroom]
        ~ play_sound("move")
        -> day_1_bathroom
    ++ [Balcony]
        ~ play_sound("move")
        -> day_1_balcony
    ++ [Kitchen]
        ~ play_sound("move")
        -> day_1_kitchen
    ++ [Stay here]
- -> act

=== day_1_bathroom
~ background("bathroom")
{day_1_bathroom > 1: -> act | -> enter}

= enter
Stella.Narrator: I step inside the small Japanese style bathroom.
Stella.Narrator: It has a deep bathtub, an open shower, a toilet, and a vanity.
Stella.Narrator: The whole thing is a little in need of cleaning, and very in need of decluttering.
Stella.Narrator: I really would have rather taken a long bath last night, but I will have to a settle for a quick shower right now.
-> act

= act
+ {!investigate_bathroom or !take_shower or !brush_teeth or !fix_hair or !apply_makeup} [Act]
    ** (investigate_bathroom) [Investigate bathroom]
        The vanity countertop is covered in water damaged books and rarely used makeup products.
        Stella.Narrator: I should clean this up when I have more time.
    ** (mirror) [Look in the mirror]
        ~ show("stella", "center")
        Stella.Narrator: It's just me.
    ** (take_shower) [Take a shower]
        Stella.Narrator: I tiredly strip off my pajamas and step into the shower.
        ~ hide("stella")
        // Play move SFX, shower SFX.
        Stella.Narrator: I turn the handle and let the cold water immediately wash over me.
        Stella.Narrator: For a while I simply enjoy the feeling.
        Stella: I suppose I should actually clean up.
        Stella.Slow: . . .
        Stella: There we go.
        // Play move SFX, end shower SFX
        Stella.Narrator: I shut off the shower, step out, towel myself off, and wrap myself in a very comfortable bathrobe.
        Stella.Narrator: I think it was a gift from Lillie.
         ~ show("stella", "center")
    ** (brush_teeth) {take_shower} [Brush teeth]
    ** (fix_hair) {brush_teeth} [Fix hair]
    ** (apply_makeup) {fix_hair} [Apply makeup]
        Stella.Narrator: No time.
    ++ [Do nothing]
+ [Move]
    ++ [Bedroom]
        ~ play_sound("move")
        -> day_1_bedroom
    ++ [Kitchen]
        ~ play_sound("move")
        -> day_1_kitchen
    ++ [Stay here]
- -> act

=== day_1_kitchen
~ background("kitchen")
{day_1_kitchen > 1: -> act | -> enter}

= enter
Stella.Narrator: I walk into the kitchen...
-> act

= act
+ {!aquire_toast or !toast_toast or !eat_toast} [Act]
    ** (aquire_toast) [Aquire toast]
    ** (toast_toast) {aquire_toast} [Toast toast]
    ** (eat_toast) {toast_toast} [Eat toast]
    ++ [Do nothing]
+ [Move]
    ++ [Bedroom]
        ~ play_sound("move")
        -> day_1_bedroom
    ++ [Bathroom]
        ~ play_sound("move")
        -> day_1_bathroom
    ++ [Apartment Hallway]
        Stella: I do not want to leave this way.
    ++ [Stay here]
- -> act

= sitting_on_balcony
Balconies r nice :D
-> END

=== day_1_balcony
~ background("balcony")
~ play_music("balcony")
{day_1_balcony > 1: -> act | -> enter}

= enter
Stella.Narrator: I step onto the balcony and breathe in the crisp autumn air.
Stella.Narrator: It's nice.
Stella.Narrator: The view of the city past the railing is pretty in the midmorning light. I should really put some furniture out here.
-> act

= act
+ [Act]
    ++ [Sit on the railing]
        {day_1_bedroom.act.get_dressed: -> sitting_on_balcony}
        Stella.Narrator: I should really shower and put some actual clothes on first.
        +++ [Sit anyway] -> sitting_on_balcony
        +++ [Do not] -> act
    ++ [Do nothing]
* {day_1_bedroom.act.talk} [Talk]
    Stella: There is still nobody here but me.
    Stella: It almost makes me actually want to leave home.
+ [Move]
    ++ [Bedroom]
        ~ play_sound("move")
        -> day_1_bedroom
    ++ [Stay here]
- -> act

= sitting_on_balcony
~ show("stella", "center")
+ [Jump]
+ [Do not]
- Balconies r nice :D
Game is over, for now!
-> END
