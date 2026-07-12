=== day_1_dream
~ clear()
~ background(none)
~ play_music("dream")
Dream: You float formless in the space between.
Dream: It is quiet. It is comfortable. It is vast beyond comprehension.
Dream: And it is so, so empty.
Dream: You have been alone for so very long.
Dream: You float in that vast expanse for what might have been an instant or an eternity.
Dream: Then, you notice a small mote of light floating in that endless void.
Dream: It is so close, how did you not notice it before?
Dream: It feels familiar. It feels warm.
Dream: You reach out to it.
Dream: Light.
Dream: Blinding.
Dream: Nothingness becomes everything.
Dream: A woman stands before you. She holds a gleaming blade aloft.
// Show glowing light
Dream: Women, standing, and blades are new to you. Yet, you know them.
Dream: The blade of light falls.
Dream: Death, too, is new to you.
Dream: Yet, you know it is coming for you all the same.
// Show aborted slashing animation
// Play sound effect
Stella.Dream: Not like this.
~ show_date()
-> day_1_bedroom

=== day_1_bedroom
~ background("bedroom")
{day_1_bedroom > 1: -> act | -> enter}

= enter
~ play_music("unimplimented") // background noise
Stella.Narrator: I open my eyes and find myself lying halfway off my bed.
Stella.Narrator: Somehow, every one of my blankets are discarded on the floor on the other side of the room.
Stella.Narrator: The first thing I notice about my body is that I feel sweaty and unusually sick.
Stella.Narrator: At least the bright morning sun coming through the window helps.
Stella: I don't wanna to get up.
~ delay(0.2)
~ play_music("typhoon_parade")
Stella.Narrator: An alarm clock starts blaring mid 2010's J-pop as if in direct rejection of my plea.
Stella: Ughhhh.
Stella.Narrator: I only set it to go off on days I have to work, which means...
Stella: I could just call in sick.
Stella: What're they going to do, fire me?
Stella[Slow]: ...
Stella.Narrator: I can't remember if I'm working at the hospital or the cafe today.
Stella: Whoever they are.
Stella.Narrator: The alarm continues to go off.
Stella.Narrator: I can't even shut it off from here because it's all the way across the room, exactly where I left it.
Stella.Narrator: Specifically to ensure I had to actually get out of bed in the morning.
Stella.Narrator: It's getting louder.
Stella.Narrator: I breifly concider blowing it up.
Stella.Narrator: But eventually, I resolve myself to fate.
Stella.Narrator: I could use a cold shower anyway. Might help with the nausea.
Stella.Narrator: I pull myself out of bed, stretch, and finally cross the room to turn off that cursed alarm clock.
~ delay(0.3)
// ~ stop_music()
~ play_sound("click_2")
~ delay(0.3)
~ play_music("morning")
Stella.Narrator: I take a breath while running my fingers through blonde hair.
Stella.Narrator: It's damp from sweat and, somehow, even messier than normal.
Stella.Narrator: I need to check where I'm supposed to be today. And get out of these sweaty pajamas and into the shower.
-> act

= act
~ play_music("morning")
+ [Act]
    ** (investigate_bedroom) [Investigate bedroom]
        The bedroom is well decorated and comfortable, but a massive pile of discarded clothes sits on the floor.
        Stella.Narrator: I think this is everything I own.
        *** (pick_up_clothes) [Pick up clothes]
            ~ add_item("clothes_pile", 1)
            Stella put all of the clothes in her pocket.
        *** [Do not]
            This is where they belong.
            Stella.Narrator: I might put them away properly later.
    ** (sleep_in) [Go back to bed]
        Stella: Maybe I can sleep for just for a bit longer.
        // Play music, background noise
        ~ clear()
        ~ delay(3)
        Stella.Narrator: If I stay in bed too long I'll fall back asleep without an alarm.
        Stella.Narrator: Again.
        ~ auto()
        _
        ~ delay(3)
        ~ play_music("morning")
        ~ background("bedroom")
        Stella.Narrator: I better get up.
    ** (check_wall_calendar) [Check wall calendar]
        The dessert themed wall calendar has a tasty-yet-incriminating ice cream cone drawn next to the month name: July.
        It hasn't been changed in three months.
        Stella.Slow: ...
    ** (check_phone) {check_wall_calendar} [Check phone calendar]
        Ignoring several unread messages, Stella checks the calendar app on her phone.
        It thankfully auto-updates when schedules are posted.
        Stella: Right, the cafe. {sleep_in: I'm already late... | I definately am going to be a bit late. }
    ++ {!investigate_bedroom or !sleep_in or !check_wall_calendar or !check_phone} [Do nothing]
    ++ ->
        There's nothing left to do here.
+ [Talk]
    Stella: There's nobody here but me, right?
+ [Move]
    ++ [Bathroom]
        ~ play_sound("move")
        -> day_1_bathroom
    ++ [Balcony]
        ~play_sound("move")
        -> day_1_balcony
    ++ [Kitchen]
        Unimplimented.
    ** (nowhere) [Nowhere]
        You're already there.
    ++ {nowhere} [Nowhere]
- -> act

=== day_1_bathroom
~ background("bathroom")
{day_1_bathroom > 1: -> act | -> enter}

= enter
Enter bathroom.
-> act

= act
+ [Act]
+ [Talk]
+ [Move]
    ++ [Bedroom]
        ~ play_sound("move")
        -> day_1_bedroom
    ++ [Nowhere]
- -> act

=== day_1_balcony
~ background("balcony")
~ play_music("balcony")
{day_1_balcony > 1: -> act | -> enter}

= enter
Stella.Narrator: I step onto the balcony and breath in the crisp autumn air.
Stella.Narrator: It's nice.
Stella.Narrator: The view of the city past the railing is pretty in the midmorning light. I should really put some furniture out here.
-> act

= act
+ [Act]
    ++ [Sit on the railing]
        {false: -> sitting_on_balcony}
        Stella.Narrator: I should really shower and put some actual clothes on first.
        +++ [Sit anyway] -> sitting_on_balcony
        +++ [Do not] -> act
    ++ [Do nothing]
+ [Talk]
    Stella: Nobody to talk to. Actually makes me kinda want to leave home.
+ [Move]
    ++ [Bedroom]
        ~ play_sound("move")
        -> day_1_bedroom
    ++ [Nowhere]
- -> act

= sitting_on_balcony
Balconies r nice :D
-> END