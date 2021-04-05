﻿namespace PandocGui.CliWrapper.Command
{
    public abstract class PandocCommandWithOptionsGenerator : PandocExecutableCommandGenerator
    {
        public IPandocCommandGenerator CommandGenerator { get; }

        protected PandocCommandWithOptionsGenerator(IPandocCommandGenerator commandGenerator)
        {
            this.CommandGenerator = commandGenerator;
        }
    }
}