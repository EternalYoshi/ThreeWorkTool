using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class Mission : DefaultWrapper
    {

        public int Unknown00;
        public int DataOffset;
        public int ID;
        public string WeirdDescriptiveString;
        public int Unknown44;

        public int P1PointCharacterID;
        public int P1PointCharacterAIFlag;
        public int P1PointCharacterAssistType;
        public int P1PointCharacterMeterHealthRegen;
        public float P1PointCharacterHealthPercentage;

        public int P1Assist1CharID;
        public int P1Assist1CharAIFlag;
        public int P1Assist1CharAssistType;
        public int P1Assist1CharMeterHealthRegen;
        public float P1Assist1CharHealthPercentage;

        public int Unknown100;

        public int P1Assist2CharID;
        public int P1Assist2CharAIFlag;
        public int P1Assist2CharAssistType;
        public int P1Assist2CharMeterHealthRegen;
        public float P1Assist2CharHealthPercentage;

        public int P2PointCharacterID;
        public int P2PointCharacterAIFlag;
        public int P2PointCharacterAssistType;
        public int P2PointCharacterMeterHealthRegen;
        public float P2PointCharacterHealthPercentage;

        public int P2Assist1CharID;
        public int P2Assist1CharAIFlag;
        public int P2Assist1CharAssistType;
        public int P2Assist1CharMeterHealthRegen;
        public float P2Assist1CharHealthPercentage;

        public int P2Assist2CharID;
        public int P2Assist2CharAIFlag;
        public int P2Assist2CharAssistType;
        public int P2Assist2CharMeterHealthRegen;
        public float P2Assist2CharHealthPercentage;

        public int MovePartPointer;
        public string MovePartString;
        //I have no idea what is in this section. It's largely zeroes, floating points that equal to 1, and at the end pointers to the following sections.

        public string ComboListString;
        public int ComboListFlagA;
        public int MissionConditionFlag;
        public int[] AnmChrMoveIDList;

        #region Mission Properties

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1PointCharacter
        {

            get
            {
                return P1PointCharacterID;
            }
            set
            {
                P1PointCharacterID = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1PointAssist
        {

            get
            {
                return P1PointCharacterAssistType;
            }
            set
            {
                P1PointCharacterAssistType = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1AIFlag
        {

            get
            {
                return P1PointCharacterAIFlag;
            }
            set
            {
                P1PointCharacterAIFlag = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1ResourceRegen
        {

            get
            {
                return P1PointCharacterMeterHealthRegen;
            }
            set
            {
                P1PointCharacterMeterHealthRegen = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public float Player1HealthPercent
        {

            get
            {
                return P1PointCharacterHealthPercentage;
            }
            set
            {
                P1PointCharacterHealthPercentage = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1Middle
        {

            get
            {
                return P1Assist1CharID;
            }
            set
            {
                P1Assist1CharID = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1MiddleAssist
        {

            get
            {
                return P1Assist1CharAssistType;
            }
            set
            {
                P1Assist1CharAssistType = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1MiddleAIFlag
        {

            get
            {
                return P1Assist1CharAIFlag;
            }
            set
            {
                P1Assist1CharAIFlag = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1MiddleResourceRegen
        {

            get
            {
                return P1Assist1CharMeterHealthRegen;
            }
            set
            {
                P1Assist1CharMeterHealthRegen = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public float Player1MiddleHealthPercent
        {

            get
            {
                return P1Assist1CharHealthPercentage;
            }
            set
            {
                P1Assist1CharHealthPercentage = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1Anchor
        {

            get
            {
                return P1Assist2CharID;
            }
            set
            {
                P1Assist2CharID = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1AnchorAssist
        {

            get
            {
                return P1Assist2CharAssistType;
            }
            set
            {
                P1Assist2CharAssistType = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1AnchorAIFlag
        {

            get
            {
                return P1Assist2CharAIFlag;
            }
            set
            {
                P1Assist2CharAIFlag = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public int Player1AnchorResourceRegen
        {

            get
            {
                return P1Assist2CharMeterHealthRegen;
            }
            set
            {
                P1Assist2CharMeterHealthRegen = value;
            }
        }

        [Category("Player1"), ReadOnlyAttribute(false)]
        public float Player1AnchorHealthPercent
        {

            get
            {
                return P1Assist2CharHealthPercentage;
            }
            set
            {
                P1Assist2CharHealthPercentage = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2PointCharacter
        {

            get
            {
                return P2PointCharacterID;
            }
            set
            {
                P2PointCharacterID = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2PointAssist
        {

            get
            {
                return P2PointCharacterAssistType;
            }
            set
            {
                P2PointCharacterAssistType = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2AIFlag
        {

            get
            {
                return P2PointCharacterAIFlag;
            }
            set
            {
                P2PointCharacterAIFlag = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2ResourceRegen
        {

            get
            {
                return P2PointCharacterMeterHealthRegen;
            }
            set
            {
                P2PointCharacterMeterHealthRegen = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public float Player2HealthPercent
        {

            get
            {
                return P2PointCharacterHealthPercentage;
            }
            set
            {
                P2PointCharacterHealthPercentage = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2Middle
        {

            get
            {
                return P2Assist1CharID;
            }
            set
            {
                P2Assist1CharID = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2MiddleAssist
        {

            get
            {
                return P2Assist1CharAssistType;
            }
            set
            {
                P2Assist1CharAssistType = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2MiddleAIFlag
        {

            get
            {
                return P2Assist1CharAIFlag;
            }
            set
            {
                P2Assist1CharAIFlag = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2MiddleResourceRegen
        {

            get
            {
                return P2Assist1CharMeterHealthRegen;
            }
            set
            {
                P2Assist1CharMeterHealthRegen = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public float Player2MiddleHealthPercent
        {

            get
            {
                return P2Assist1CharHealthPercentage;
            }
            set
            {
                P2Assist1CharHealthPercentage = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2Anchor
        {

            get
            {
                return P2Assist2CharID;
            }
            set
            {
                P2Assist2CharID = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2AnchorAssist
        {

            get
            {
                return P2Assist2CharAssistType;
            }
            set
            {
                P2Assist2CharAssistType = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2AnchorAIFlag
        {

            get
            {
                return P2Assist2CharAIFlag;
            }
            set
            {
                P2Assist2CharAIFlag = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public int Player2AnchorResourceRegen
        {

            get
            {
                return P2Assist2CharMeterHealthRegen;
            }
            set
            {
                P2Assist2CharMeterHealthRegen = value;
            }
        }

        [Category("Player2"), ReadOnlyAttribute(false)]
        public float Player2AnchorHealthPercent
        {

            get
            {
                return P2Assist2CharHealthPercentage;
            }
            set
            {
                P2Assist2CharHealthPercentage = value;
            }
        }

        [Category("ComboList"), ReadOnlyAttribute(false)]
        public int[] ComboMoves
        {
            get
            {
                return AnmChrMoveIDList;
            }
            set
            {
                AnmChrMoveIDList = value;
            }
        }

        [Category("ComboList"), ReadOnlyAttribute(false)]
        public int UnknownMoveListFlagA
        {

            get
            {
                return ComboListFlagA;
            }
            set
            {
                ComboListFlagA = value;
            }
        }

        [Category("ComboList"), ReadOnlyAttribute(false)]
        public int MissionClearCondition
        {

            get
            {
                return MissionConditionFlag;
            }
            set
            {
                MissionConditionFlag = value;
            }
        }
        
        #endregion

    }
}
