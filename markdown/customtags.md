# Creating Custom Tags
RueI supports creating custom rich text tags. 

all rich text tags should ultimately derive from [RichTextTag](../api/RueI.Parsing.RichTextTag.html). if you need certain parameter types, there are a few abstract subclasses that automatically parse their parameters.
 - for measurement tags (e.g <line-height=5em>) use 