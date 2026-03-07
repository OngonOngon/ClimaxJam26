using UnityEngine;

namespace Dubinci
{
    public enum CommandType
    {
        Shoot,
        ShootAll,
        Build
    }

    [CreateAssetMenu(fileName = "CommandSO", menuName = "Scriptable Objects/CommandSO")]
    public class CommandSO : ScriptableObject
    {
        public void changeType(CommandType type)
        {
            this.type = type;
        }

        public CommandType type;
    }
}
