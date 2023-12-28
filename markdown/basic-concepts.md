# Basic Concepts
this serves as a basic introduction to RueI concepts and functionality.

the most basic components of RueI are [Elements](../api/RueI.Elements.Element.html), which act like individual hints with their own content. elements are contained within [DisplayBases](../api/RueI.Displays.DisplayBases.html). elements are not tied to a player (so you can have an instance of an element in multiple displays at once), but displays are. 

finally, every ReferenceHub (player) in RueI has an associated [DisplayCore](../api/RueI.Displays.DisplayCore.html), which manages all of the displays for a player and combines the elements in them to one hint. every DisplayCore also has a [Scheduler](../api/RueI.Displays.Scheduling.Scheduler), enabling synchronized delayed updates.

there are two element types that RueI provides out of the box:

- [SetElement](../api/RueI.Elements.SetElement.html): a very simple element with settable content
- [DynamicElement](../api/RueI.Elements.DynamicElement.html): an element that, when the display is updated/refreshed, gets its content by calling a function.

every time you update something in RueI, such as the content of an element, you must update the DisplayCore by calling [DisplayCore.Update()](../api/RueI.Displays.DisplayCore.html#RueI_Displays_DisplayCore_Update_System_Int32_). you can get the DisplayCore for a player by calling [DisplayCore.Get(ReferenceHub)](../api/RueI.Displays.DisplayCore.html#RueI_Displays_DisplayCore_Get_ReferenceHub_). while it's easy to forget to do this, this is unfortunately necessary.
