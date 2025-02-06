
using System.Collections.Generic;

public class Team 
{
    // default team ID is 0
    private int _id = 0;
    private string _name; // name of team
    private int _count;
    private List<Player> _members; // member of team

    public int ID { get => _id; set => _id = value; }

    public string Name { get => _name; set => _name = value; }

    /// <summary>
    /// How many players are here?
    /// </summary>
    public int CountMembers { get => _count; set => _count = value; }

    /// <summary>
    /// List of players which is this team member
    /// </summary>
    public List<Player> MemberList { get => _members; set => _members = value; }

}
