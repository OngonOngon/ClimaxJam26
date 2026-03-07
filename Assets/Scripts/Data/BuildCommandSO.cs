using System;
using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Build", menuName = "Scriptable Objects/Command Build")]
    public class BuildCommandSO : CommandSO
    {
        public TowerSO tower;
        public event Action<TowerSO> OnBuildCommand;

        public override bool TryCommand(string text)
        {
            if (base.TryCommand(text))
            {
                OnBuildCommand?.Invoke(tower);
                return true;
            }
            else
                return false;
        }

        [ContextMenu("Build")]
        public void PlayBuild()
        {
            OnBuildCommand?.Invoke(tower);
        }
    }
}
