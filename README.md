# C4Editor
# What it is
This application is a visual editor for creating C4 Diagrams.  C4 diagrams are useful for documenting system architectures and interconnection of the different modules and users.

The C4 method of describing architecture as code was originally presented by Simon Brown and is very well described at [https://c4model.com/](https://c4model.com/ "https://c4model.com/")

There are implementations of creating these diagrams using many different programming languages.

I had a need on a project for a more viaual approach to creating these diagrams.  This project is the result.

It reads and writes JSON files, which can be stored in version control.  

Internally, this application converts the JSON file into a Plant-UML script that is shown on the right hand panel.

This is the editor
![./editor.png](editor.png "editor.png")

In order to add a module or edit properties of existing modules, right click on the tree on the left
![./ContextMenu.png](ContextMenu.png "ContextMenu.png")

Below is a sample of a property sheet
![./PropertyEditor.png](PropertyEditor.png "PropertyEditor.png")

By selecting the "Source" tab, you can see the Plant-UML source code
![./PlantUmlText.png](PlantUmlText.png "PlantUmlText.png")


# How to build it
Download the source

Open Visual Studio 2022 - The project compiles using .Net 6

Execute the application

