using UnityEngine;


// interface for the data element of the cell data.  these will be monobehaviors that impart particular gridmap related functionality to a gameobject
// one example includes the transform which places an object (such as a building or a tile) on the map, including snapping the object's world coordinates
// another example includes a collision component which 
//maybe this is dumb... not sure what the point of this is... 
//nah, it's to allow the map to store stuff of any kind in a cell data for a game object
//actually why don't we just do it as a bunch of maps instead?
//much later comment -- i think we actually did this... and then this still remained.  


//i think there's something wrong with this -- we're using it in two ways:
//one way is that it represents a cell that can have multiple objects in it
//another way is the grid transform or really any object that can be mapped
//this double duty could cause a problem later.

//or maybe the point is this -- each cell just has a reference to one or more mappable objects.  so this doesn't represent a cell -- it's a reference within a cell
//i think this makes the most sense.

//the whole point is that each of our sub maps is of a type of a particular one of the igridmapable things.

//much much later comment: yeah, but isn't everything that's mapable just going to be a grid transform?  i think that makes the most sense...
//gridtransform is already extremely flexible plus we can put whatever options we want on there...

public interface IGridMapable
{
    //how about get rect...
    RectInt GetRect();
    Color GetMinimapColor();
    int GetMiniMapPriority();
}
