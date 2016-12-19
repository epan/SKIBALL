# TreeRandomizer
A Unity plugin which allows to randomize trees.

This editor plugin allows you to generate multiple different trees from one tree (if the tree is made with the unity tree system).

Basically this plugin duplicates the tree and randomizes its seed. 
It also references the materials of the original/template tree to minimize disk space usage.

## Usage
To open the window navigate to Tools -> Tree Randomizer

Now drag a tree from the project window into the 'Tree Template' slot. The tree count specifies how many trees will be generated.
Finally press 'Generate Trees'.
The trees will be generated to a sub-folder named 'Generated'.

If the trees appear to have no texture, try these things (should never happen, just in case):
- reimport terrain (in project explorer, rightclick and then press "Reimport")
- reimport trees 
- change seed of textureless trees again