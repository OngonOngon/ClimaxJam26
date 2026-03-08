using System;
using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "Scriptable Objects/Command Modifier")]
    public class BuildModifierSO : CommandSO
    {
        public ModifierCellSO modifier;
        public event Action<ModifierCellSO> OnBuildCommand;

        public override void RunCommand()
        {
            OnBuildCommand?.Invoke(modifier);
        }

        [ContextMenu("Build")]
        public void PlayBuild()
        {
            RunCommand();
        }
    }
}