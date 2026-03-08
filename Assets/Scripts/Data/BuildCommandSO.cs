using System;
using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Build", menuName = "Scriptable Objects/Command Build")]
    public class BuildCommandSO : CommandSO
    {
        public TowerSO tower;
        public event Action<TowerSO> OnBuildCommand;

        public override void RunCommand()
        {
            OnBuildCommand?.Invoke(tower);
        }

        [ContextMenu("Build")]
        public void PlayBuild()
        {
            RunCommand();
        }
    }
}