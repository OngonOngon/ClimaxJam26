using System;
using System.Linq;
using UnityEngine;

namespace Dubinci
{
    public enum CommandType
    {
        Shoot,
        ShootAll,
        Build,
        Upgrade
    }

    [CreateAssetMenu(fileName = "Command", menuName = "Scriptable Objects/Command")]
    public class CommandSO : ScriptableObject
    {
        public string text;
        public CommandType type;

        public event Action OnCommand;
        public event Func<bool> Validate;

        public void changeType(CommandType type)
        {
            this.type = type;
        }

        public virtual void RunCommand()
        {
            OnCommand?.Invoke();
        }

        public bool ExactMatch(string text)
        {
            return text == this.text;
        }

        public virtual bool ValidCommand()
        {
            if (Validate == null)
                return false;
            return Validate.GetInvocationList()
                        .Cast<Func<bool>>()
                        .All(f => f());
        }

        [ContextMenu("Play")]
        public void Play()
        {
            OnCommand?.Invoke();
        }
    }
}
