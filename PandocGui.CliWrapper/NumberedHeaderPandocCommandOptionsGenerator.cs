﻿namespace PandocGui.CliWrapper
{
    public class NumberedHeaderPandocCommandOptionsGenerator: PandocCommandWithOptionsGenerator
    {
        public NumberedHeaderPandocCommandOptionsGenerator(IPandocCommandGenerator commandGenerator) : base(commandGenerator)
        {
        }

        public override string GetCommand(string sourcePath) => $"{CommandGenerator.GetCommand(sourcePath)} -N";
    }
}