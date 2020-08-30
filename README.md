# Graph-Editor

The program is aimed to generate the graphs using the Model-View architechure.
It provides the users with simple functionality of the program to create vertices
and edges. Also, it enables the user to move the graphs and dynamically change it.

![Alt text](graph.jpg?raw=true "Optional Title")

## Usage

The user can :

- Create a new vertices by holding SHIFT and clicking in the window (this will 
automatically assign the vertex with the ID and it be selected)
- Hold down CTRL key and click any vertex to create an edge between the selected 
and pointed vertex.
- The same action (previous one) will remove the edge.
- Delete the selected vertex by pressing DELETE key.

The graph editor also has menu section that has

- New -  Delete all vertices and edges, i.e. start over with a new graph.
- Open - Prompt the user for a filename, then read the graph from that file.
- Save - Saves the graph to a file. It can save to the file if it was opened 
previously
- Quit - Exit the application

## Usage

```
dotnet run
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.
