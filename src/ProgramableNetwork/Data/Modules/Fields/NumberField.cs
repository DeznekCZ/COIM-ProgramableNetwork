﻿using Mafi.Unity.UiFramework;
using Mafi.Unity.UiFramework.Components;
using Mafi.Unity.UserInterface;
using System;

namespace ProgramableNetwork
{
    public class NumberField<T> : IField
    {
        public NumberField(string id, string name, T defaultValue)
        {
            Id = id;
            Name = name;
            Default = defaultValue;
        }

        public string Id { get; }
        public string Name { get; }

        public int Size => 20;

        public T Default { get; }

        public void Init(ControllerInspector inspector, WindowView parentWindow, StackContainer fieldContainer, UiBuilder uiBuider, Module module, Action updateDialog)
        {
            fieldContainer.SetStackingDirection(StackContainer.Direction.LeftToRight);
            fieldContainer.SetHeight(20);

            uiBuider
                .NewTxt("name")
                .SetParent(fieldContainer, true)
                .SetWidth(180)
                .SetHeight(20)
                .SetText(Name)
                .AppendTo(fieldContainer);

            var numberEditor = uiBuider
                .NewTxtField("value")
                .SetParent(fieldContainer, true)
                .SetWidth(200)
                .SetHeight(20)
                .SetText("0")
                .AppendTo(fieldContainer);

            numberEditor.SetOnValueChangedAction(() =>
            {
                if (typeof(T) == typeof(int))
                {
                    if (int.TryParse(numberEditor.GetText(), out int value))
                    {
                        module.NumberData[Name] = value;
                    }
                }
                else if (typeof(T) == typeof(long))
                {
                    if (long.TryParse(numberEditor.GetText(), out long value))
                    {
                        module.StringData[Name] = value.ToString();
                    }
                }
            });

            if (typeof(T) == typeof(int))
            {
                if (module.NumberData.TryGetValue(Name, out var num))
                {
                    numberEditor.SetText(num.ToString());
                }
                else
                {
                    numberEditor.SetText(Default.ToString());
                    module.NumberData[Name] = (int)(object)Default;
                }
            }
            else if (typeof(T) == typeof(long))
            {
                if (module.StringData.TryGetValue(Name, out var num))
                {
                    numberEditor.SetText(num);
                }
                else
                {
                    numberEditor.SetText(Default.ToString());
                    module.StringData[Name] = Default.ToString();
                }
            }
        }
    }
}