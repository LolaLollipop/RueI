# Getting Started
this guide assumes that you have a basic level of knowledge with c#.

there are three main ways of using RueI: as a 'hard' dependency, as a 'soft' dependency, or through reflection. 
- with a hard dependency, you directly referencr RueI and use it in place of hints. the advantages of this are that it gives you type safety, it is significantly faster, and it is just overall much easier to use. however, if someone does *not* have RueI installed, then your plugin will stop working. this makes it ideal for plugins that rely on RueI heavily, or if you're creating a plugin for your own use.
- a soft dependency still means that you have to reference RueI, but you also provide alternatives using hints and switch to those alternatives if RueI isn't detected, ideally using a dependency injection model. 
- using reflection, you can dynamically add support for RueI, meaning that you don't have it reference it at all. reflection, however, is slow and does not provide any of the compile-time guarantees of the first two options, making it much harder to work with. 

the way that you should use RueI depends on who your plugin is for and what it's doing. if you're making a private plugin or want to make use of all of the features of RueI, then you should make it a hard dependency. if your plugin is small and/or extensively uses hints, you should use it as a soft dependency. finally, if your plugin only uses hints occassionally, reflection is the best choice.

this guide will cover how to use RueI using the first option, as a normal hard dependency. 

### installing RueI
