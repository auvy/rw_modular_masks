Instead of usual boring vulture masks, this mod makes them more individualized.

New individualized masks consist of multiple parts: eye cutouts, lower mask, forehead, horns, side horns. (Paint is not yet done.)

The ID of vulture and/or mask entity determines the pieces of which the new mask will consist of.

The mod then chooses the pieces and bakes them into a texture, 9 times for each rotation.

This also works for king vulture masks, but not for elite scavengers or scavenger king.

My weird design choices:
- some sprites are duplicated to make the mask variety closer to vanilla.
- the texture is merged into one pixel by pixel, and all 9 rotations *are not* stored as one atlas, but as *separate atlases of one rotation*. This is because i don't know how atlases work in the game.
- technically CWT should make sure there is no remaining masks left, but *i do not remember* if the mask sprites get deleted when the mask is gone/vulture dies for memory. Most likely not.
- the sprites are dumped into StreamingAssets during playtime as .pngs for debugging purposes.
- the scavenger paint was supposed to be implemented, for elite scavengers, but i do not know how to do it. Maybe like king vulture arrows idk.

TODO:
- split the code by modules
- stop the debugging output of textures into streamingassets
- custom scavenger paint maybe someday idk

