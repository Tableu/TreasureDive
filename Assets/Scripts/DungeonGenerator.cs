/*using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;




// 
//     This is a variation on Hunt-and-Kill where the initial maze has rooms carved out of
//     it, instead of being completely flat.
//     Optional Parameters
//     rooms: List(List(tuple, tuple))
//         A list of lists, containing the top-left and bottom-right grid coords of where
//         you want rooms to be created. For best results, the corners of each room should
//         have odd-numbered coordinates.
//     grid: i8[H,W]
//         A pre-built maze array filled with one, or many, rooms.
//     hunt_order: String ['random', 'serpentine']
//         Determines how the next cell to hunt from will be chosen. (default 'random')
//     
public class DungeonRooms {
    public const int RANDOM = 1;

    public const int SERPENTINE = 2;
    // highest-level method that implements the maze-generating algorithm
    //         Returns:
    //             np.array: returned matrix
    //         
    private string[][] grid;
    private int H;
    private int W;
    public string[][] generate(int h, int w)
    {
        H = h;
        W = w;
        // define grid and rooms
        this.grid = this.backup_grid.copy();
        this._carve_rooms(this.rooms);
        // select start position for algorithm
        var current = this._choose_start();
        this.grid[current[0]][current[1]] = 0;
        // perform many random walks, to fill the maze
        var num_trials = 0;
        while (current != (-1, -1)) {
            this._walk(current);
            current = this._hunt(num_trials);
            num_trials += 1;
        }
        // fix any unconnected wall sections
        this.reconnect_maze();
        return this.grid;
    }
    
    // Open up user-defined rooms in a maze.
    //         Args:
    //             rooms (list): collection of room positions (corners of large rooms)
    //         Returns: None
    //         
    public virtual object _carve_rooms(object rooms) {
        if (rooms == null) {
            return;
        }
        foreach (var room in rooms) {
            try {
                var _tup_1 = room;
                var top_left = _tup_1.Item1;
                var bottom_right = _tup_1.Item2;
                this._carve_room(top_left, bottom_right);
                this._carve_door(top_left, bottom_right);
            } catch (Exception) {
                // If the user tries to create an invalid room, it is simply ignored.
            }
        }
    }
    
    // Open up a single user-defined room in a maze.
    //         Args:
    //             top_left (tuple): position of top-left cell in the room
    //             bottom_right (tuple): position of bottom-right cell in the room
    //         Returns: None
    //         
    public virtual object _carve_room(object top_left, object bottom_right) {
        foreach (var row in Enumerable.Range(top_left[0], bottom_right[0] + 1 - top_left[0])) {
            foreach (var col in Enumerable.Range(top_left[1], bottom_right[1] + 1 - top_left[1])) {
                this.grid[row,col] = 0;
            }
        }
    }
    
    // Open up a single door in a user-defined room, IF that room does not already have a whole wall of doors.
    //         Args:
    //             top_left (tuple): position of top-left cell in the room
    //             bottom_right (tuple): position of bottom-right cell in the room
    //         Returns: None
    //         
    public virtual object _carve_door(object top_left, object bottom_right) {
        var even_squares = (from i in (top_left.ToList() + bottom_right.ToList())
            where i % 2 == 0
            select i).ToList();
        if (even_squares.Count > 0) {
            return;
        }
        // find possible doors on all sides of room
        var possible_doors = new List<object>();
        var odd_rows = (from i in Enumerable.Range(top_left[0] - 1, bottom_right[0] + 2 - (top_left[0] - 1))
            where i % 2 == 1
            select i).ToList();
        var odd_cols = (from i in Enumerable.Range(top_left[1] - 1, bottom_right[1] + 2 - (top_left[1] - 1))
            where i % 2 == 1
            select i).ToList();
        if (top_left[0] > 2) {
            possible_doors += zip(new List<object> {
                (top_left[0] - 1)
            } * odd_rows.Count, odd_rows);
        }
        if (top_left[1] > 2) {
            possible_doors += zip(odd_cols, new List<object> {
                (top_left[1] - 1)
            } * odd_cols.Count);
        }
        if (bottom_right[0] < this.grid.shape[0] - 2) {
            possible_doors += zip(new List<object> {
                (bottom_right[0] + 1)
            } * odd_rows.Count, odd_rows);
        }
        if (bottom_right[1] < this.grid.shape[1] - 2) {
            possible_doors += zip(odd_cols, new List<object> {
                (bottom_right[1] + 1)
            } * odd_cols.Count);
        }
        var door = choice(possible_doors);
        this.grid[door[0],door[1]] = 0;
    }
    
    // This is a standard random walk. It must start from a visited cell.
    //         And it completes when the current cell has no unvisited neighbors.
    //         Args:
    //             start (tuple): position of a grid cell
    //         Returns: None
    //         
    public virtual object _walk(object start) {
        if (this.grid[start[0],start[1]] == 0) {
            var current = start;
            var unvisited_neighbors = this._find_neighbors(current[0], current[1], this.grid, true);
            while (unvisited_neighbors.Count > 0) {
                var neighbor = choice(unvisited_neighbors);
                this.grid[neighbor[0],neighbor[1]] = 0;
                this.grid[(neighbor[0] + current[0]) / 2,(neighbor[1] + current[1]) / 2] = 0;
                current = neighbor;
                unvisited_neighbors = this._find_neighbors(current[0], current[1], this.grid, true);
            }
        }
    }
    
    // Based on how this algorithm was configured, choose hunt for the next starting point.
    //         Args:
    //             count (int): number of trials left to hunt for
    //         Returns:
    //             tuple: position of next cell
    //         
    public virtual object _hunt(object count) {
        if (this._hunt_order == SERPENTINE) {
            return this._hunt_serpentine(count);
        } else {
            return this._hunt_random(count);
        }
    }
    
    // Select the next cell to walk from, randomly.
    //         Args:
    //             count (int): number of trials left to hunt for
    //         Returns:
    //             tuple: position of next cell
    //         
    public virtual object _hunt_random(object count) {
        if (count >= this.H * this.W) {
            return (-1, -1);
        }
        return (randrange(1, this.H, 2), randrange(1, this.W, 2));
    }
    
    // Select the next cell to walk from by cycling through every grid cell in order.
    //         Args:
    //             count (int): number of trials left to hunt for
    //         Returns:
    //             tuple: position of next cell
    //         
    public virtual object _hunt_serpentine(object count) {
        var cell = (1, 1);
        var found = false;
        while (!found) {
            cell = (cell[0], cell[1] + 2);
            if (cell[1] > this.W - 2) {
                cell = (cell[0] + 2, 1);
                if (cell[0] > this.H - 2) {
                    return (-1, -1);
                }
            }
            if (this.grid[cell[0]][cell[1]] == 0 && this._find_neighbors(cell[0], cell[1], this.grid, true).Count > 0) {
                found = true;
            }
        }
        return cell;
    }
    
    // Choose a random starting location, that is not already inside a room.
    //         If no such room exists, the input grid was invalid.
    //         Returns:
    //             tuple: arbitrarily-selected room in the maze, that is not part of a big room
    //         
    public Vector2Int _choose_start() {
        var current = new Vector2Int(randrange(1, this.H, 2), randrange(1, this.W, 2));
        var LIMIT = this.H * this.W * 2;
        var num_tries = 1;
        // keep looping until you find an unvisited cell
        while (num_tries < LIMIT) {
            current = (randrange(1, this.H, 2), randrange(1, this.W, 2));
            if (this.grid[current[0]][current[1]] == 1) {
                break;
            }
            num_tries += 1;
        }
        Debug.Assert(num_tries < LIMIT);
        Debug.Assert("The grid input to DungeonRooms was invalid.");
        return current;
    }
    
    // If a maze is not fully connected, open up walls until it is.
    //         Returns: None
    //         
    public virtual object reconnect_maze() {
        this._fix_disjoint_passages(this._find_all_passages());
    }
    
    // Place all connected passage cells into a set. Disjoint passages will be in different sets.
    //         Returns:
    //             list: collection of paths
    //         
    public virtual object _find_all_passages() {
        var passages = new List<object>();
        // go through all cells in the maze
        foreach (var r in Enumerable.Range(0, Convert.ToInt32(Math.Ceiling(Convert.ToDouble(this.grid.shape[0] - 1) / 2))).Select(_x_1 => 1 + _x_1 * 2)) {
            foreach (var c in Enumerable.Range(0, Convert.ToInt32(Math.Ceiling(Convert.ToDouble(this.grid.shape[1] - 1) / 2))).Select(_x_2 => 1 + _x_2 * 2)) {
                var ns = this._find_unblocked_neighbors((r, c));
                var current = new HashSet<object>(ns + new List<object> {
                    (r, c)
                });
                // determine which passage(s) the current neighbors belong in
                var found = false;
                foreach (var _tup_1 in passages.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
                    var i = _tup_1.Item1;
                    var passage = _tup_1.Item2;
                    var intersect = current.intersection(passage);
                    if (intersect.Count > 0) {
                        passages[i] = passages[i].union(current);
                        found = true;
                        break;
                    }
                }
                // the current neighbors might be a disjoint set
                if (!found) {
                    passages.append(current);
                }
            }
        }
        return this._join_intersecting_sets(passages);
    }
    
    // All passages in a maze should be connected
    //         Args:
    //             disjoint_passages (list): collections of paths in the maze which do not fully connect
    //         Returns: None
    //         
    public virtual object _fix_disjoint_passages(List<List<Vector2Int>> disjoint_passages) {
        while (disjoint_passages.Count > 1) {
            var found = false;
            while (!found) {
                // randomly select a cell in the first passage
                var cell = choice(disjoint_passages[0].ToList());
                var neighbors = this._find_neighbors(cell[0], cell[1], this.grid);
                // determine if that cell has a neighbor in any other passage
                foreach (var passage in disjoint_passages[1]) {
                    var intersect = (from c in neighbors
                        where passage.Contains(c)
                        select c).ToList();
                    // if so, remove the dividing wall, and combine the two passages
                    if (intersect.Count > 0) {
                        var mid = this._midpoint(intersect[0], cell);
                        this.grid[mid[0],mid[1]] = 0;
                        disjoint_passages[0] = disjoint_passages[0].union(passage);
                        disjoint_passages.remove(passage);
                        found = true;
                        break;
                    }
                }
            }
        }
    }
    
    // Find all the grid neighbors of the current position; visited, or not.
    //         Args:
    //             posi (tuple): position of the cell of interest
    //         Returns:
    //             list: all the open, unblocked neighbor cells that you can go to from this one
    //         
    public virtual object _find_unblocked_neighbors(Vector2Int posi) {
        var _tup_1 = posi;
        var r = _tup_1.y;
        var c = _tup_1.x;
        var ns = new List<Vector2Int>();
        if (r > 1 && this.grid[r - 1][c] != null && this.grid[r - 2][c] != null) {
            ns.Add(new Vector2Int(r - 2, c));
        }
        if (r < this.grid.Length - 2 && this.grid[r + 1][c] != null && this.grid[r + 2][c] != null) {
            ns.Add(new Vector2Int(r + 2, c));
        }
        if (c > 1 && this.grid[r][c - 1] != null && this.grid[r][c - 2] != null) {
            ns.Add(new Vector2Int(r, c - 2));
        }
        if (c < this.grid[0].Length - 2 && this.grid[r][c + 1] != null && this.grid[r][c + 2] != null) {
            ns.Add(new Vector2Int(r, c + 2));
        }
        ShuffleList(ns);
        return ns;
    }
    
    // combine sets that have non-zero intersections
    //         Args:
    //             list_of_sets (list): sets of paths that do not interesect
    //         Returns:
    //             list: a combined collection of paths, now intersection
    //         
    public virtual object _join_intersecting_sets(Vector2Int[][] list_of_sets) {
        foreach (var i in Enumerable.Range(0, list_of_sets.Length - 1)) {
            if (list_of_sets[i] == null) {
                continue;
            }
            foreach (var j in Enumerable.Range(i + 1, list_of_sets.Length - (i + 1))) {
                if (list_of_sets[j] == null) {
                    continue;
                }
                var intersect = list_of_sets[i].Intersect(list_of_sets[j]);
                if (intersect.Any()) {
                    list_of_sets[i] = list_of_sets[i].Union(list_of_sets[j]).ToArray();
                    list_of_sets[j] = null;
                }
            }
        }
        return list_of_sets.Where(l => l != null).ToList().ToList();
    }
    
    // Find the wall cell between to passage cells
    //         Args:
    //             a (tuple): position of one cell
    //             b (tuple): position of a different cell
    //         Returns:
    //             tuple: position of a cell half-way between the two given
    //         
    public Vector2Int _midpoint(Vector2Int a, Vector2Int b) {
        return new Vector2Int((a[0] + b[0]) / 2, (a[1] + b[1]) / 2);
    }
    
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count-1; i++)
        {
            T tmp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}*/