using Papper.Internal;
using Papper.Tests.Mappings;
using Papper.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Papper.Tests
{
    public class OptimizerTests
    {
        [Theory]
        [InlineData(OptimizerType.Block, typeof(MultiTest_DB), 1, "MultiTest_DB", 100, "", "", "")]
        [InlineData(OptimizerType.Block, typeof(DB_MotionHMI), 2, "DB_MotionHMI", 100, "HMI.TogglePLC", "HMI.SelectedLine", "HMI.MotionLine[8].Txt.DirectionRight")]
        [InlineData(OptimizerType.Items, typeof(DB_MotionHMI), 3, "DB_MotionHMI", 100, "HMI.TogglePLC", "HMI.SelectedLine", "HMI.MotionLine[8].Txt.DirectionRight")]
        [InlineData(OptimizerType.Block, typeof(DB_MotionHMI), 1, "DB_MotionHMI", 100, "HMI.MotionLine[8].MotionState.Final_Position[0]", "HMI.MotionLine[8].MotionState.Display_Order[0]", "HMI.MotionLine[8].AccessRightReqFromHmiId")]
        [InlineData(OptimizerType.Items, typeof(DB_MotionHMI), 3, "DB_MotionHMI", 100, "HMI.MotionLine[8].MotionState.Final_Position[0]", "HMI.MotionLine[8].MotionState.Display_Order[0]", "HMI.MotionLine[8].AccessRightReqFromHmiId")]
        [InlineData(OptimizerType.Block, typeof(DB_MotionHMI), 1, "DB_MotionHMI", 100, "HMI.MotionLine[8].MotionState.Final_Position", "HMI.MotionLine[8].MotionState.Display_Order", "HMI.MotionLine[8].AccessRightReqFromHmiId")]
        [InlineData(OptimizerType.Items, typeof(DB_MotionHMI), 2, "DB_MotionHMI", 100, "HMI.MotionLine[8].MotionState.Final_Position", "HMI.MotionLine[8].MotionState.Display_Order", "HMI.MotionLine[8].AccessRightReqFromHmiId")]
        [InlineData(OptimizerType.Block, typeof(BoolArrayTestMapping), 1, "BOOL_ARRAY_TEST_MAPPING", 100, "NotFull", "Full")]
        [InlineData(OptimizerType.Items, typeof(BoolArrayTestMapping), 2, "BOOL_ARRAY_TEST_MAPPING", 100, "NotFull", "Full")]
        public void CreateRawBlocksByGiveOptimizerTest(OptimizerType optimzerType, Type type, int expectedPartitions, string mappingName, int size, params string[] values)
        {
            var tree = new PlcMetaDataTree();
            var mapping = PlcObjectResolver.GetMapping(mappingName, tree, type);
            var variables = new Dictionary<string, OperationItem>();
            PlcObjectResolver.AddPlcObjects(mapping, variables, values);

            var optimizer = OptimizerFactory.CreateOptimizer(optimzerType);
            var rawBlocks = optimizer.CreateRawReadOperations(mapping.Name, mapping.Selector, variables, size).ToList();

            Assert.Equal(expectedPartitions, rawBlocks.Count);
        }
    }
}
