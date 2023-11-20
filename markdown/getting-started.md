# Getting Started
this guide assumes that you have a basic level of knowledge with c#.

there are two main ways of using RueI: through a normal dependency or through reflection. 
- with a normal dependency, you include RueI as a dependency in your project. the advantages of this are that it gives you type safety, it is significantly faster, and it is just overall much easier to use. however, if someone does *not* have RueI installed, then your plugin will stop working. this makes it ideal for plugins that rely on RueI heavily, or if you're creating a plugin for your own use.
- with reflection, you can dynamically add support for RueI, meaning that your plugin works even if someone doesn't have RueI installed.

this guide will cover how to use RueI using the first option, as a normal dependency. 

### installing RueI
