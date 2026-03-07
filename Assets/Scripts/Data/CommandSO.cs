using System;
using UnityEngine;

namespace Dubinci
{
    public enum CommandType
    {
        Shoot,
        ShootAll,
        Build
    }

    [CreateAssetMenu(fileName = "Command", menuName = "Scriptable Objects/Command")]
    public class CommandSO : ScriptableObject
    {
        public string text;
        public CommandType type;

        public event Action OnCommand;

        public void changeType(CommandType type)
        {
            this.type = type;
        }

        public virtual bool TryCommand(string text)
        {
            if (this.text == text)
            {
                OnCommand?.Invoke();
                return true;
            }
            return false;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            OnCommand?.Invoke();
        }
    }
}
