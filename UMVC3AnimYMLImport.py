bl_info = {
"name": "Yoshi's Test Script",
"description":"For importing UMVC3 animations.",
"author":"Eternal Yoshi",
"version":(0,0,2),
"blender":(3,0,0),
"location": "File > Import",
"warning": "Set your Armature to the Bind Pose before applying animations or risk having a bad time.",
"category":"Import-Export",
}

import bpy, os, sys, time, traceback, mathutils, re, subprocess

def install_module(module):
    subprocess.check_call(['pip', 'install', module])
    print(f"The module {module} was installed")

import pip
pip.main(['install', 'pyyaml', '--user'])

try: 
    import yaml
except ImportError:
    print("The yaml module appears to be missing so let's attempt to install it.\n")
    install_module('yaml')
    import yaml



# Import Code.
from bpy_extras.io_utils import ImportHelper

from bpy_extras.io_utils import ExportHelper
from mathutils import Quaternion

class data():
   def __init__(self, X, Y, Z, W):
        self.X = X
        self.Y = Y
        self.Z = Z
        self.W = W

class Keyframes():
    def __init__(self, Frame, TrackType, BoneID, data):
        self.Frame = Frame
        self.TrackType = TrackType
        self.BoneID = BoneID
        self.data = data

class LMTM3AEntry:
      yaml_loader = yaml.SafeLoader
      yaml_tag = u'!LMTM3AEntry'

      def __init__(self, version, Name, FrameCount, LoopFrame, KeyFrames):
        self.version = version
        self.Name = Name
        self.FrameCount = FrameCount
        self.LoopFrame = LoopFrame
        self.KeyFrames = KeyFrames

def LMTM3AEntry_constructor(loader: yaml.SafeLoader, node: yaml.nodes.MappingNode) -> LMTM3AEntry:
  """Construct a LMTM3AEntry."""
  return LMTM3AEntry(**loader.construct_mapping(node))

def get_loader():
  """Add constructors to PyYAML loader."""
  loader = yaml.SafeLoader
  loader.add_constructor("!LMTM3AEntry", LMTM3AEntry_constructor)
  return loader

#From the SSBU Animation exporter.        
def get_heirarchy_order(bone_list: list[bpy.types.PoseBone]) -> list[bpy.types.PoseBone]:
    root_bone: bpy.types.PoseBone = None
    for bone in bone_list:
        if bone.parent is None:
            root_bone = bone
            break
    return [root_bone] + [c for c in root_bone.children_recursive if c in bone_list]

class Location():
    def __init__(self, x, y, z):
        self.x: float = x
        self.y: float = y
        self.z: float = z
    def __repr__(self) -> str:
        return f'[{self.x=}, {self.y=}, {self.z=}]'

class Rotation():
    def __init__(self, w, x, y, z):
        self.w: float = w
        self.x: float = x
        self.y: float = y
        self.z: float = z
    def __repr__(self) -> str:
        return f'[{self.w=}, {self.x=}, {self.y=}, {self.z=}]'

class Scale():
    def __init__(self, x, y, z):
        self.x: float = x
        self.y: float = y
        self.z: float = z
    def __repr__(self) -> str:
        return f'[{self.x=}, {self.y=}, {self.z=}]'
    
def keyframeCount(bone, start, end) -> int:

    N_keyframes = 0

    action = bpy.data.actions["ArmatureAction"]
    for fcu in action.fcurves:
        print(fcu.data_path + " channel " + str(fcu.array_index))     
        bonename = str(bone.name)      
        for keyframe in fcu.keyframe_points:
            if str(bone.name) in fcu.data_path:
                print(keyframe.co) #coordinates x,y
                N_keyframes += 1



    return N_keyframes

#Double Checking the bone name for conflicts.
def NameChecker(index, fcu, bonename) -> bool:
    #pdb.set_trace()
#This is inteded to remove naming issues with the data path that result in redundant/incorrect keys being inserted for the bone.
    if bonename == "jnt_1":
        if "jnt_10" in fcu.data_path:
            return False
        if "jnt_11" in fcu.data_path:
            return False
        if "jnt_12" in fcu.data_path:
            return False
        if "jnt_13" in fcu.data_path:
            return False
        if "jnt_14" in fcu.data_path:
            return False
        if "jnt_15" in fcu.data_path:
            return False
        if "jnt_16" in fcu.data_path:
            return False
        if "jnt_17" in fcu.data_path:
            return False
        if "jnt_18" in fcu.data_path:
            return False
        if "jnt_19" in fcu.data_path:
            return False
        return True

    if bonename == "jnt_2":  
        if "jnt_20" in fcu.data_path:
            return False
        if "jnt_21" in fcu.data_path:
            return False
        if "jnt_22" in fcu.data_path:
            return False
        if "jnt_23" in fcu.data_path:
            return False
        if "jnt_24" in fcu.data_path:
            return False
        if "jnt_25" in fcu.data_path:
            return False
        if "jnt_26" in fcu.data_path:
            return False
        if "jnt_27" in fcu.data_path:
            return False
        if "jnt_28" in fcu.data_path:
            return False
        if "jnt_29" in fcu.data_path:
            return False
        return True

    if bonename == "jnt_3":   
        if "jnt_30" in fcu.data_path:
            return False
        if "jnt_31" in fcu.data_path:
            return False
        if "jnt_32" in fcu.data_path:
            return False
        if "jnt_33" in fcu.data_path:
            return False
        if "jnt_34" in fcu.data_path:
            return False
        if "jnt_35" in fcu.data_path:
            return False
        if "jnt_36" in fcu.data_path:
            return False
        if "jnt_37" in fcu.data_path:
            return False
        if "jnt_38" in fcu.data_path:
            return False
        if "jnt_39" in fcu.data_path:
            return False
        return True
        
    if bonename == "jnt_4":
        if "jnt_40" in fcu.data_path:
            return False
        if "jnt_41" in fcu.data_path:
            return False
        if "jnt_42" in fcu.data_path:
            return False
        if "jnt_43" in fcu.data_path:
            return False
        if "jnt_44" in fcu.data_path:
            return False
        if "jnt_45" in fcu.data_path:
            return False
        if "jnt_46" in fcu.data_path:
            return False
        if "jnt_47" in fcu.data_path:
            return False
        if "jnt_48" in fcu.data_path:
            return False
        if "jnt_49" in fcu.data_path:
            return False
        return True
        
    if bonename == "jnt_5": 
        if "jnt_50" in fcu.data_path:
            return False
        if "jnt_51" in fcu.data_path:
            return False
        if "jnt_52" in fcu.data_path:
            return False
        if "jnt_53" in fcu.data_path:
            return False
        if "jnt_54" in fcu.data_path:
            return False
        if "jnt_55" in fcu.data_path:
            return False
        if "jnt_56" in fcu.data_path:
            return False
        if "jnt_57" in fcu.data_path:
            return False
        if "jnt_58" in fcu.data_path:
            return False
        if "jnt_59" in fcu.data_path:
            return False
        return True     

    if bonename == "jnt_6":  
        if "jnt_60" in fcu.data_path:
            return False
        if "jnt_61" in fcu.data_path:
            return False
        if "jnt_62" in fcu.data_path:
            return False
        if "jnt_63" in fcu.data_path:
            return False
        if "jnt_64" in fcu.data_path:
            return False
        if "jnt_65" in fcu.data_path:
            return False
        if "jnt_66" in fcu.data_path:
            return False
        if "jnt_67" in fcu.data_path:
            return False
        if "jnt_68" in fcu.data_path:
            return False
        if "jnt_69" in fcu.data_path:
            return False 
        return True   

    if bonename == "jnt_7":
        if "jnt_70" in fcu.data_path:
            return False
        if "jnt_71" in fcu.data_path:
            return False
        if "jnt_72" in fcu.data_path:
            return False
        if "jnt_73" in fcu.data_path:
            return False
        if "jnt_74" in fcu.data_path:
            return False
        if "jnt_75" in fcu.data_path:
            return False
        if "jnt_76" in fcu.data_path:
            return False
        if "jnt_77" in fcu.data_path:
            return False
        if "jnt_78" in fcu.data_path:
            return False
        if "jnt_79" in fcu.data_path:
            return False 
        return True                          

    if bonename == "jnt_8":  
        if "jnt_80" in fcu.data_path:
            return False
        if "jnt_81" in fcu.data_path:
            return False
        if "jnt_82" in fcu.data_path:
            return False
        if "jnt_83" in fcu.data_path:
            return False
        if "jnt_84" in fcu.data_path:
            return False
        if "jnt_85" in fcu.data_path:
            return False
        if "jnt_86" in fcu.data_path:
            return False
        if "jnt_87" in fcu.data_path:
            return False
        if "jnt_88" in fcu.data_path:
            return False
        if "jnt_89" in fcu.data_path:
            return False
        return True
        
    if bonename == "jnt_9":    
        if "jnt_90" in fcu.data_path:
            return False
        if "jnt_91" in fcu.data_path:
            return False
        if "jnt_92" in fcu.data_path:
            return False
        if "jnt_93" in fcu.data_path:
            return False
        if "jnt_94" in fcu.data_path:
            return False
        if "jnt_95" in fcu.data_path:
            return False
        if "jnt_96" in fcu.data_path:
            return False
        if "jnt_97" in fcu.data_path:
            return False
        if "jnt_98" in fcu.data_path:
            return False
        if "jnt_99" in fcu.data_path:
            return False 
        return True           

    if bonename == "jnt_10":
        if "jnt_100" in fcu.data_path:
            return False    
        if "jnt_110" in fcu.data_path:
            return False
        if "jnt_101" in fcu.data_path:
            return False
        if "jnt_102" in fcu.data_path:
            return False
        if "jnt_103" in fcu.data_path:
            return False
        if "jnt_104" in fcu.data_path:
            return False
        if "jnt_105" in fcu.data_path:
            return False
        if "jnt_106" in fcu.data_path:
            return False
        if "jnt_107" in fcu.data_path:
            return False
        if "jnt_108" in fcu.data_path:
            return False
        if "jnt_109" in fcu.data_path:
            return False 
        return True    

    if bonename == "jnt_11":    
        if "jnt_110" in fcu.data_path:
            return False
        if "jnt_111" in fcu.data_path:
            return False
        if "jnt_112" in fcu.data_path:
            return False
        if "jnt_113" in fcu.data_path:
            return False
        if "jnt_114" in fcu.data_path:
            return False
        if "jnt_115" in fcu.data_path:
            return False
        if "jnt_116" in fcu.data_path:
            return False
        if "jnt_117" in fcu.data_path:
            return False
        if "jnt_118" in fcu.data_path:
            return False
        if "jnt_119" in fcu.data_path:
            return False 
        return True    

    if bonename == "jnt_12":    
        if "jnt_120" in fcu.data_path:
            return False
        if "jnt_121" in fcu.data_path:
            return False
        if "jnt_122" in fcu.data_path:
            return False
        if "jnt_123" in fcu.data_path:
            return False
        if "jnt_124" in fcu.data_path:
            return False
        if "jnt_125" in fcu.data_path:
            return False
        if "jnt_126" in fcu.data_path:
            return False
        if "jnt_127" in fcu.data_path:
            return False
        if "jnt_128" in fcu.data_path:
            return False
        if "jnt_129" in fcu.data_path:
            return False 
        return True    
    
    if bonename == "jnt_13":    
        if "jnt_130" in fcu.data_path:
            return False
        if "jnt_131" in fcu.data_path:
            return False
        if "jnt_132" in fcu.data_path:
            return False
        if "jnt_133" in fcu.data_path:
            return False
        if "jnt_134" in fcu.data_path:
            return False
        if "jnt_135" in fcu.data_path:
            return False
        if "jnt_136" in fcu.data_path:
            return False
        if "jnt_137" in fcu.data_path:
            return False
        if "jnt_138" in fcu.data_path:
            return False
        if "jnt_139" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_14":    
        if "jnt_140" in fcu.data_path:
            return False
        if "jnt_141" in fcu.data_path:
            return False
        if "jnt_142" in fcu.data_path:
            return False
        if "jnt_143" in fcu.data_path:
            return False
        if "jnt_144" in fcu.data_path:
            return False
        if "jnt_145" in fcu.data_path:
            return False
        if "jnt_146" in fcu.data_path:
            return False
        if "jnt_147" in fcu.data_path:
            return False
        if "jnt_148" in fcu.data_path:
            return False
        if "jnt_149" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_15":    
        if "jnt_150" in fcu.data_path:
            return False
        if "jnt_151" in fcu.data_path:
            return False
        if "jnt_152" in fcu.data_path:
            return False
        if "jnt_153" in fcu.data_path:
            return False
        if "jnt_154" in fcu.data_path:
            return False
        if "jnt_155" in fcu.data_path:
            return False
        if "jnt_156" in fcu.data_path:
            return False
        if "jnt_157" in fcu.data_path:
            return False
        if "jnt_158" in fcu.data_path:
            return False
        if "jnt_159" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_16":    
        if "jnt_160" in fcu.data_path:
            return False
        if "jnt_161" in fcu.data_path:
            return False
        if "jnt_162" in fcu.data_path:
            return False
        if "jnt_163" in fcu.data_path:
            return False
        if "jnt_164" in fcu.data_path:
            return False
        if "jnt_165" in fcu.data_path:
            return False
        if "jnt_166" in fcu.data_path:
            return False
        if "jnt_167" in fcu.data_path:
            return False
        if "jnt_168" in fcu.data_path:
            return False
        if "jnt_169" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_17":    
        if "jnt_170" in fcu.data_path:
            return False
        if "jnt_171" in fcu.data_path:
            return False
        if "jnt_172" in fcu.data_path:
            return False
        if "jnt_173" in fcu.data_path:
            return False
        if "jnt_174" in fcu.data_path:
            return False
        if "jnt_175" in fcu.data_path:
            return False
        if "jnt_176" in fcu.data_path:
            return False
        if "jnt_177" in fcu.data_path:
            return False
        if "jnt_178" in fcu.data_path:
            return False
        if "jnt_179" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_18":    
        if "jnt_180" in fcu.data_path:
            return False
        if "jnt_181" in fcu.data_path:
            return False
        if "jnt_182" in fcu.data_path:
            return False
        if "jnt_183" in fcu.data_path:
            return False
        if "jnt_184" in fcu.data_path:
            return False
        if "jnt_185" in fcu.data_path:
            return False
        if "jnt_186" in fcu.data_path:
            return False
        if "jnt_187" in fcu.data_path:
            return False
        if "jnt_188" in fcu.data_path:
            return False
        if "jnt_189" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_19":    
        if "jnt_190" in fcu.data_path:
            return False
        if "jnt_191" in fcu.data_path:
            return False
        if "jnt_192" in fcu.data_path:
            return False
        if "jnt_193" in fcu.data_path:
            return False
        if "jnt_194" in fcu.data_path:
            return False
        if "jnt_195" in fcu.data_path:
            return False
        if "jnt_196" in fcu.data_path:
            return False
        if "jnt_197" in fcu.data_path:
            return False
        if "jnt_198" in fcu.data_path:
            return False
        if "jnt_199" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_20":    
        if "jnt_200" in fcu.data_path:
            return False
        if "jnt_201" in fcu.data_path:
            return False
        if "jnt_202" in fcu.data_path:
            return False
        if "jnt_203" in fcu.data_path:
            return False
        if "jnt_204" in fcu.data_path:
            return False
        if "jnt_205" in fcu.data_path:
            return False
        if "jnt_206" in fcu.data_path:
            return False
        if "jnt_207" in fcu.data_path:
            return False
        if "jnt_208" in fcu.data_path:
            return False
        if "jnt_209" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_21":    
        if "jnt_210" in fcu.data_path:
            return False
        if "jnt_211" in fcu.data_path:
            return False
        if "jnt_212" in fcu.data_path:
            return False
        if "jnt_213" in fcu.data_path:
            return False
        if "jnt_214" in fcu.data_path:
            return False
        if "jnt_215" in fcu.data_path:
            return False
        if "jnt_216" in fcu.data_path:
            return False
        if "jnt_217" in fcu.data_path:
            return False
        if "jnt_218" in fcu.data_path:
            return False
        if "jnt_219" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_22":    
        if "jnt_220" in fcu.data_path:
            return False
        if "jnt_221" in fcu.data_path:
            return False
        if "jnt_222" in fcu.data_path:
            return False
        if "jnt_223" in fcu.data_path:
            return False
        if "jnt_224" in fcu.data_path:
            return False
        if "jnt_225" in fcu.data_path:
            return False
        if "jnt_226" in fcu.data_path:
            return False
        if "jnt_227" in fcu.data_path:
            return False
        if "jnt_228" in fcu.data_path:
            return False
        if "jnt_229" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_23":    
        if "jnt_230" in fcu.data_path:
            return False
        if "jnt_231" in fcu.data_path:
            return False
        if "jnt_232" in fcu.data_path:
            return False
        if "jnt_233" in fcu.data_path:
            return False
        if "jnt_234" in fcu.data_path:
            return False
        if "jnt_235" in fcu.data_path:
            return False
        if "jnt_236" in fcu.data_path:
            return False
        if "jnt_237" in fcu.data_path:
            return False
        if "jnt_238" in fcu.data_path:
            return False
        if "jnt_239" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_24":    
        if "jnt_240" in fcu.data_path:
            return False
        if "jnt_241" in fcu.data_path:
            return False
        if "jnt_242" in fcu.data_path:
            return False
        if "jnt_243" in fcu.data_path:
            return False
        if "jnt_244" in fcu.data_path:
            return False
        if "jnt_245" in fcu.data_path:
            return False
        if "jnt_246" in fcu.data_path:
            return False
        if "jnt_247" in fcu.data_path:
            return False
        if "jnt_248" in fcu.data_path:
            return False
        if "jnt_249" in fcu.data_path:
            return False 
        return True

    if bonename == "jnt_25":    
        if "jnt_250" in fcu.data_path:
            return False
        if "jnt_251" in fcu.data_path:
            return False
        if "jnt_252" in fcu.data_path:
            return False
        if "jnt_253" in fcu.data_path:
            return False
        if "jnt_254" in fcu.data_path:
            return False
        if "jnt_255" in fcu.data_path:
            return False
        if "jnt_256" in fcu.data_path:
            return False
        return True

    return True

def ApplyTheTrack(Track, obj, joint, jointEdit):
    
    Frame = 0
    Bone = Track[0]['BoneID']
    
    #Checks if the bone exists on the Armature in the scene and will skip if it doesn't.
    if bpy.data.objects["Armature"].data.bones.get(f'jnt_{Bone}') is None:
        return
    
    for ID, Keyframe in enumerate(Track):
    
        if(Track[0]['TrackType'] == "localscale"):
            print("A scale Track.")
            #Do Stuff here.
            try:
                Frame = int(Keyframe['Frame'])
                joint.scale = (float(Keyframe['data']['X']), float(Keyframe['data']['Y']), float(Keyframe['data']['Z']))
                #print(joint.scale)
                
                obj.keyframe_insert(data_path='pose.bones["%s"].%s' %
                                (f'jnt_{Bone}', "scale"), frame=(Frame))
                
                                    
            except Exception as sc:
                print("Problem applying scalar keyframe.",sc, "\n", traceback.format_exc())
                continue          
      
        if(Track[0]['TrackType'] == "localposition"):
            print("A Translation Track.")
            #Do Stuff here.
            try:
                Frame = int(Keyframe['Frame'])
                joint.location = (float(Keyframe['data']['X']), float(Keyframe['data']['Y']), float(Keyframe['data']['Z']))
                #print(joint.scale)
                
                obj.keyframe_insert(data_path='pose.bones["%s"].%s' %
                                (f'jnt_{Bone}', "location"), frame=(Frame))
                
                                    
            except Exception as sc:
                print("Problem applying translation keyframe.",sc, "\n", traceback.format_exc())
                continue              
      
      
        if(Track[0]['TrackType'] == "localrotation"):
            print("A Rotation Track.")
            obj = bpy.data.objects["Armature"]
            joint = obj.pose.bones[f'jnt_{Bone}']
            print(str(joint))
            #Do Stuff here.
            try:
                Frame = int(Keyframe['Frame'])
                Thing = mathutils.Quaternion((float(Keyframe['data']['W']), float(Keyframe['data']['X']), float(Keyframe['data']['Y']), float(Keyframe['data']['Z'])))
                print(Thing)                                                
                joint.rotation_quaternion = Thing
                print(joint.rotation_quaternion)
                
                #print(obj.pose.bones[0].rotation_mode)
                
                obj.keyframe_insert(data_path=joint.path_from_id("rotation_quaternion"), frame=(Frame))
                
                                    
            except Exception as sc:
                print("Problem applying rotational keyframe.",sc, "\n", traceback.format_exc())
                continue            

def readM3AanimationData(self,context,filepath):
    global AnimName; AnimName = ""
    GroupCount = 0
    global FrameCount; FrameCount = 0
    NodeCount = 0
    global AnimGroups; AnimGroups = {}
    keycount = 0
    os.system('cls')
    
    print(self.files); print(filepath)
    
    #Stores the object selected.
    obj = bpy.context.active_object

    #Changes the blender mode to Pose Mode.
    bpy.ops.object.mode_set(mode = 'POSE', toggle=False)    
        
    #This block forces ALL bones to use quaternion rotation & sets each bone to the Identity Matrix.
    for bone in obj.pose.bones:
        bone.matrix_basis.identity()
        bone.rotation_mode = 'QUATERNION'
    bpy.data.objects["Armature"].rotation_mode = 'QUATERNION'
    
    try:
        #Opens the yml and deserializes.
        data_loaded = yaml.load(open(filepath, "rb"), Loader=get_loader())
        
        print(data_loaded.version)
        print(data_loaded.Name)
        print(data_loaded.FrameCount)
        print(data_loaded.LoopFrame)
        keycount = 0
        counter = 0
        
        #Adjust the animation timeline to fit the animation and set the current frame to zero.
        bpy.data.scenes["Scene"].frame_start = 0
        bpy.data.scenes["Scene"].frame_end = data_loaded.FrameCount
        bpy.context.scene.frame_set(0)
        
        pose_bones = bpy.data.objects['Armature'].pose.bones
        NewJointName = ""
        for x in range(len(pose_bones)):
            print(pose_bones[x])
            
        #for idx, data_loaded.KeyFrames in enumerate(data_loaded.Keyframes):
                #if data_loaded.KeyFrames[idx]['BoneID'] == 255:
                    #continue
        
        
        #Checks if the bone exists on the Armature in the scene and will skip if it doesn't.
        #if bpy.data.objects["Armature"].data.bones.get(f'jnt_{BID}') is None:
            #contnniue
        
        Track = []
        #This part applies it to the scene.
        for id, Keyframe in enumerate(data_loaded.KeyFrames):
            
            BID = data_loaded.KeyFrames[0]['BoneID']

            #Checks if the bone exists on the Armature in the scene and will skip if it doesn't.
            if bpy.data.objects["Armature"].data.bones.get(f'jnt_{BID}') is None:
                continue

            #Selects the bone and deselects everything else.
            bpy.context.active_object.select_set(False)
            for obj in bpy.context.selected_objects:
                bpy.context.view_layer.objects.active = obj

            obj = bpy.data.objects["Armature"]
            joint = obj.pose.bones[f'jnt_{BID}']
            jointEdit = bpy.data.armatures["Armature"].bones[f'jnt_{BID}'].matrix

            #If the animation range is lower than the current frame, expand the animation range to accomodate.
            if int(bpy.data.scenes["Scene"].frame_end < data_loaded.FrameCount):
                bpy.data.scenes["Scene"].frame_end = data_loaded.FrameCount
            
            print(Keyframe['BoneID'])
            if (id != 0):
                #Thing.
                if(Keyframe['BoneID'] != PrevBoneID or Keyframe['TrackType'] != PrevTrackType):
                    #Apply Stuff here.
                                            
                    #Go To function when Track is used to apply all keyframes to specified bone.
                    ApplyTheTrack(Track, obj, joint, jointEdit)
                        
                    #Then we empty the Track.
                    del Track[:]
                        
                    #Then continue as usual.
                    Track.append(Keyframe)
                    PrevTrackType = Keyframe['TrackType']
                    PrevBoneID = Keyframe['BoneID']
             
                else:      
                    Track.append(Keyframe)
                    PrevTrackType = Keyframe['TrackType']
                    PrevBoneID = Keyframe['BoneID']
    
            else:
                Track.append(Keyframe)    
                PrevTrackType = Keyframe['TrackType']
                PrevBoneID = Keyframe['BoneID']
                        
                        
                        
        
        
    
    except Exception as e:
        print("An error occured.",e, "\n", traceback.format_exc())    
        
def WriteM3AanimationData(context,filepath):
    global AnimName; AnimName = ""
    GroupCount = 0
    global FrameCount; FrameCount = 0
    NodeCount = 0
    global AnimGroups; AnimGroups = {}
    keycount = 0
    os.system('cls')
    
    #Stores the object selected.
    obj = bpy.context.active_object
    TrueKeys = []
    
    #Changes the blender mode to Pose Mode.
    bpy.ops.object.mode_set(mode = 'POSE', toggle=False)

    try:

        #Gets info from the animation timeline to acquire the starting and ending frames of the animation. First one is always zero.
        FirstFrame = bpy.data.scenes["Scene"].frame_start
        FrameCount = bpy.data.scenes["Scene"].frame_end
        bpy.context.scene.frame_set(0)

        print("First frame: ", FirstFrame, "while the last frame is: ", FrameCount)

        obj = bpy.data.objects["Armature"]

        #Attempts to actually gather all of the keyframes in the scene.
        
        #Borrowed from the Smash Uitimate animation exporter because the other methods don't get the keys I want without redundant data
        #that would bloat file size.
        
        # Create value dicts ahead of time
        bone_name_to_location_values: dict[str, list[Location]] = {}
        bone_name_to_rotation_values: dict[str, list[Rotation]] = {}
        bone_name_to_scale_values: dict[str, list[Scale]] = {}
        bone_to_rel_matrix_local = {}
        reordered_pose_bones = get_heirarchy_order(list(obj.pose.bones))
        
        # Fill value dicts with default values. Not every bone will be animated, so for these the default values of a matrix basis will be needed
        for pose_bone in reordered_pose_bones:
            bone_name_to_location_values[pose_bone.name] = [Location(0.0, 0.0, 0.0) for _ in range(FirstFrame, FrameCount + 1)]
            bone_name_to_rotation_values[pose_bone.name] = [Rotation(1.0, 0.0, 0.0, 0.0) for _ in range(FirstFrame, FrameCount + 1)]
            bone_name_to_scale_values[pose_bone.name] = [Scale(1.0, 1.0, 1.0) for _ in range(FirstFrame, FrameCount + 1)]
            if pose_bone.parent: # non-root bones
                bone_to_rel_matrix_local[pose_bone] = pose_bone.parent.bone.matrix_local.inverted() @ pose_bone.bone.matrix_local
            else: # root bones
                bone_to_rel_matrix_local[pose_bone] = pose_bone.bone.matrix_local
        
        # Go through the pose bones' fcurves and store all the values at each frame.
        animated_pose_bones: set[bpy.types.PoseBone] = set()
        
        keycount = 0
        tracks = []

        for fcurve in obj.animation_data.action.fcurves:
            regex = r'pose\.bones\[\"(.*)\"\]\.(.*)'
            matches = re.match(regex, fcurve.data_path)
            if matches is None: # A fcurve in the action that isn't a bone transform, such as the user keyframing the Armature Object itself.
                operator.report(type={'WARNING'}, message=f"The fcurve with data path {fcurve.data_path} will not be exported, since it didn't match the pattern of a bone fcurve.")
                continue
            if len(matches.groups()) != 2: # TODO: Is this possible?
                operator.report(type={'WARNING'}, message=f"The fcurve with data path {fcurve.data_path} will not be exported, its format only partially matched the expected pattern of a bone fcurve.")
                continue
            bone_name = matches.groups()[0]
            transform_subtype = matches.groups()[1]
            if transform_subtype == 'location':
                for index, frame in enumerate(range(FirstFrame, FrameCount+1)):
                    if fcurve.array_index == 0:
                        bone_name_to_location_values[bone_name][index].x = fcurve.evaluate(frame)
                    elif fcurve.array_index == 1:
                        bone_name_to_location_values[bone_name][index].y = fcurve.evaluate(frame)
                    elif fcurve.array_index == 2:
                        bone_name_to_location_values[bone_name][index].z = fcurve.evaluate(frame)
            elif transform_subtype == 'rotation_quaternion':
                for index, frame in enumerate(range(FirstFrame, FrameCount+1)):
                    if fcurve.array_index == 0:
                        bone_name_to_rotation_values[bone_name][index].w = fcurve.evaluate(frame)
                    elif fcurve.array_index == 1:
                        bone_name_to_rotation_values[bone_name][index].x = fcurve.evaluate(frame)
                    elif fcurve.array_index == 2:
                        bone_name_to_rotation_values[bone_name][index].y = fcurve.evaluate(frame)
                    elif fcurve.array_index == 3:
                        bone_name_to_rotation_values[bone_name][index].z = fcurve.evaluate(frame)
            elif transform_subtype == 'scale':
                for index, frame in enumerate(range(FirstFrame, FrameCount+1)):
                    if fcurve.array_index == 0:
                        bone_name_to_scale_values[bone_name][index].x = fcurve.evaluate(frame)
                    elif fcurve.array_index == 1:
                        bone_name_to_scale_values[bone_name][index].y = fcurve.evaluate(frame)
                    elif fcurve.array_index == 2:
                        bone_name_to_scale_values[bone_name][index].z = fcurve.evaluate(frame)
            animated_pose_bone = obj.pose.bones.get(bone_name)
            if animated_pose_bone is not None:
                animated_pose_bones.add(animated_pose_bone)
                
        bone_to_world_matrix = {}
        '''
        sorted_fcurves = {}
        for i in bpy.data.actions:
            print (i.name)
            for fcu in i.fcurves:
                print (fcu)
                for keyframe in fcu.keyframe_points:
                    x, y = keyframe.co
                    print (x,y)
        '''
        action = bpy.data.actions["ArmatureAction"]
                                                
    #with open('C:\\dump.txt', 'w') as f:
        for bone in reordered_pose_bones:
            
            #f.write("\n____________________________________________________________________________________________________")
            #f.write("\nBone ID: "+str(bone.name))
            print("_________________________________________________________________________\nNow Serving Bone ID: " + str(bone.name))          
            
            #Variables for holding Location, Rotational, and Scale Data.
            FramesL = []
            KeysL = []
            KeyTypesL = []
            InterpolationsL = []

            FramesR = []
            KeysR = []
            KeyTypesR = []
            InterpolationsR = []

            FramesS = []
            KeysS = []
            KeyTypesS = []
            InterpolationsS = []


            for index, _ in enumerate(range(FirstFrame, FrameCount+1)):
                #Gets the needed values and prints them out in the console and in dump.txt.
                trans_basis_vec = bone_name_to_location_values[bone.name][index]
                trans_basis_quat = Quaternion([0, trans_basis_vec.x, trans_basis_vec.y, trans_basis_vec.z])
                rot_basis_vec = bone_name_to_rotation_values[bone.name][index]
                rot_basis_quat = Quaternion([rot_basis_vec.w, rot_basis_vec.x, rot_basis_vec.y, rot_basis_vec.z])
                scale_basis_vec = bone_name_to_scale_values[bone.name][index]
                scale_basis_quat = Quaternion([0, scale_basis_vec.x, scale_basis_vec.y, scale_basis_vec.z])
                
                #f.write("\nFrame : "+str(index))
                #Checks for actual keys on the frame... in a roundabout way.      
                for fcu in action.fcurves:
                    #Skips if the fcurve lacks the current bone name we're working with.
                    if bone.name in fcu.data_path:
                        
                        #Frame Check.
                        for keyframe in fcu.keyframe_points:                                
                            if keyframe.co[0] == index:
                                #Now For Location/Rotation/Scale.

                                #keycount += 1
                                if "location" in fcu.data_path:   
                                    #f.write("\nTranslation: "+str(trans_basis_vec))
                                    #X
                                    #if fcu.array_index == 0:
                                    #print("BINGO! A location Keyframe.")
                                    #f.write("\nTranslation: "+str(trans_basis_vec.x))

                                    #Y
                                    #if fcu.array_index == 1:
                                        #print("BINGO! A location Keyframe.")
                                        #f.write("\nTranslation: "+str(trans_basis_vec.y))                                            
                                        #keycount += 1                                        
                                    #Z
                                    if fcu.array_index == 2:
                                        if NameChecker(index, fcu, bone.name):                                                
                                            print(fcu.data_path)
                                            pt = [pt for pt in fcu.keyframe_points if pt.co[0] == index][0]
                                            InterpolationsL.append(pt.interpolation)
                                            #f.write("\nTranslation: "+str(trans_basis_vec))
                                            keycount += 1
                                            KeyedT = True
                                            
                                            FramesL.append(index)
                                            KeysL.append(trans_basis_quat)
                                            KeyTypesL.append("location")
                                            FrameText = bone.name
                                            FrameText = FrameText.replace(FrameText[:4], '')
                                            KeyToInsert = data(trans_basis_vec.x,trans_basis_vec.y,trans_basis_vec.z,0)
                                            TrueKeys.append(Keyframes(index,"localposition",int(FrameText),KeyToInsert))


                                        #gen_track(bone.name,0,bone,trans_basis_vec, index)                                                                                            
                                    
                                                                                                                                                                            
                
            #print(bone.name)
            print(len(FramesL))
            print(FramesL)
            print(len(FramesR))
            print(FramesR)
            print(len(FramesS))
            print(FramesS)                
            #print(len(Keys))
            #print(Keys)
            #print(("\n"))
            #print(Interpolations)
            #print(len(Interpolations))



        #f.write("\nKeyframe Total: "+str(keycount))    
    
        for bone in reordered_pose_bones:
            
            #f.write("\n____________________________________________________________________________________________________")
            #f.write("\nBone ID: "+str(bone.name))
            print("_________________________________________________________________________\nNow Serving Bone ID: " + str(bone.name))          
            
            #Variables for holding Location, Rotational, and Scale Data.
            FramesL = []
            KeysL = []
            KeyTypesL = []
            InterpolationsL = []

            FramesR = []
            KeysR = []
            KeyTypesR = []
            InterpolationsR = []

            FramesS = []
            KeysS = []
            KeyTypesS = []
            InterpolationsS = []


            for index, _ in enumerate(range(FirstFrame, FrameCount+1)):
                #Gets the needed values and prints them out in the console and in dump.txt.
                trans_basis_vec = bone_name_to_location_values[bone.name][index]
                trans_basis_quat = Quaternion([0, trans_basis_vec.x, trans_basis_vec.y, trans_basis_vec.z])
                rot_basis_vec = bone_name_to_rotation_values[bone.name][index]
                rot_basis_quat = Quaternion([rot_basis_vec.w, rot_basis_vec.x, rot_basis_vec.y, rot_basis_vec.z])
                scale_basis_vec = bone_name_to_scale_values[bone.name][index]
                scale_basis_quat = Quaternion([0, scale_basis_vec.x, scale_basis_vec.y, scale_basis_vec.z])
                
                #f.write("\nFrame : "+str(index))
                #Checks for actual keys on the frame... in a roundabout way.      
                for fcu in action.fcurves:
                    #Skips if the fcurve lacks the current bone name we're working with.
                    if bone.name in fcu.data_path:
                        
                        #Frame Check.
                        for keyframe in fcu.keyframe_points:                                
                            if keyframe.co[0] == index:
                                #Now For Location/Rotation/Scale.
                                        
                                if "scale" in fcu.data_path:                                      

                                    #f.write("\nRotation: "+str(rot_basis_quat)) 
                                    #W
                                    #if fcu.array_index == 0:
                                        #print("BINGO! A rotation Keyframe.")
                                        #f.write("\nRotation: "+str(rot_basis_quat.w))                                            
                                        #keycount += 1                                        
                                    #X
                                    #if fcu.array_index == 1:
                                        #print("BINGO! A rotation Keyframe.")
                                        #f.write("\nRotation: "+str(rot_basis_quat.x))                                               
                                        #keycount += 1                                        
                                    #Y
                                    #if fcu.array_index == 2:
                                        #print("BINGO! A rotation Keyframe.")
                                        #f.write("\nRotation: "+str(rot_basis_quat.y))                                               
                                        #keycount += 1                                        
                                    #Z
                                    if fcu.array_index == 3:
                                        if NameChecker(index, fcu, bone.name):                                                   
                                            print(fcu.data_path)
                                            pt = [pt for pt in fcu.keyframe_points if pt.co[0] == index][0]
                                            InterpolationsR.append(pt.interpolation)
                                            #f.write("\scale: "+str(rot_basis_quat))                                               
                                            keycount += 1
                                            KeyedR = True

                                            FramesR.append(index)
                                            KeysR.append(rot_basis_quat)
                                            KeyTypesR.append("scale")
                                            FrameText = bone.name
                                            FrameText = FrameText.replace(FrameText[:4], '')
                                            KeyToInsert = data(scale_basis_vec.x,scale_basis_vec.y,scale_basis_vec.z,1)
                                            TrueKeys.append(Keyframes(index,"localscale",int(FrameText),KeyToInsert))                                                       
                                                                                                                                                        
                                    
                                                                                                                                                                            
                
            #print(bone.name)
            print(len(FramesL))
            print(FramesL)
            print(len(FramesR))
            print(FramesR)
            print(len(FramesS))
            print(FramesS)                
            #print(len(Keys))
            #print(Keys)
            #print(("\n"))
            #print(Interpolations)
            #print(len(Interpolations))



        #f.write("\nKeyframe Total: "+str(keycount))   
    
    
    
        for bone in reordered_pose_bones:
            
            #f.write("\n____________________________________________________________________________________________________")
            #f.write("\nBone ID: "+str(bone.name))
            print("_________________________________________________________________________\nNow Serving Bone ID: " + str(bone.name))          
            
            #Variables for holding Location, Rotational, and Scale Data.
            FramesL = []
            KeysL = []
            KeyTypesL = []
            InterpolationsL = []

            FramesR = []
            KeysR = []
            KeyTypesR = []
            InterpolationsR = []

            FramesS = []
            KeysS = []
            KeyTypesS = []
            InterpolationsS = []


            for index, _ in enumerate(range(FirstFrame, FrameCount+1)):
                #Gets the needed values and prints them out in the console and in dump.txt.
                trans_basis_vec = bone_name_to_location_values[bone.name][index]
                trans_basis_quat = Quaternion([0, trans_basis_vec.x, trans_basis_vec.y, trans_basis_vec.z])
                rot_basis_vec = bone_name_to_rotation_values[bone.name][index]
                rot_basis_quat = Quaternion([rot_basis_vec.w, rot_basis_vec.x, rot_basis_vec.y, rot_basis_vec.z])
                scale_basis_vec = bone_name_to_scale_values[bone.name][index]
                scale_basis_quat = Quaternion([0, scale_basis_vec.x, scale_basis_vec.y, scale_basis_vec.z])
                
                #f.write("\nFrame : "+str(index))
                #Checks for actual keys on the frame... in a roundabout way.      
                for fcu in action.fcurves:
                    #Skips if the fcurve lacks the current bone name we're working with.
                    if bone.name in fcu.data_path:
                        
                        #Frame Check.
                        for keyframe in fcu.keyframe_points:                                
                            if keyframe.co[0] == index:
                                #Now For Scale.
                                                                                                                                                                        
                                if "rotation_quaternion" in fcu.data_path:

                                    #f.write("\nScale: "+str(scale_basis_vec))                                        
                                    #X
                                    #if fcu.array_index == 0:
                                        #print("BINGO! A scale Keyframe.")
                                        #f.write("\nScale: "+str(scale_basis_vec.x))
                                        #keycount += 1                                        
                                    #Y
                                    #if fcu.array_index == 1:
                                        #print("BINGO! A scale Keyframe.")
                                        #f.write("\nScale: "+str(scale_basis_vec.y))
                                        #keycount += 1                                        
                                    #Z
                                    if fcu.array_index == 2:
                                        if NameChecker(index, fcu, bone.name):
                                            print(fcu.data_path)
                                            pt = [pt for pt in fcu.keyframe_points if pt.co[0] == index][0]
                                            InterpolationsS.append(pt.interpolation)
                                            #f.write("\rotation_quaternion: "+str(scale_basis_vec))
                                            keycount += 1
                                            KeyedR = True

                                            FramesS.append(index)
                                            KeysS.append(scale_basis_quat)
                                            KeyTypesS.append("rotation_quaternion")
                                            FrameText = bone.name
                                            FrameText = FrameText.replace(FrameText[:4], '')
                                            KeyToInsert = data(rot_basis_vec.x,rot_basis_vec.y,rot_basis_vec.z,rot_basis_vec.w)
                                            TrueKeys.append(Keyframes(index,"localrotation",int(FrameText),KeyToInsert))                                                
                                    
                                                                                                                                                                            
                
            #print(bone.name)
            print(len(FramesL))
            print(FramesL)
            print(len(FramesR))
            print(FramesR)
            print(len(FramesS))
            print(FramesS)                
            #print(len(Keys))
            #print(Keys)
            #print(("\n"))
            #print(Interpolations)
            #print(len(Interpolations))



        #f.write("\nKeyframe Total: "+str(keycount))   
    
    
    
    #f.close()        
        
        #Prints stuff out to check.
        print(str(keycount))
        bpy.context.scene.frame_set(0)

        FinalAnim = LMTM3AEntry(1,"AnimDataID0",FrameCount,-1,TrueKeys)
        yaml.emitter.Emitter.process_tag = lambda self, *args, **kw: None
        stream = open(filepath,'w')
        yaml.dump(FinalAnim,stream,sort_keys=False)

        #Adds in important tag.
        GoodTag = "!LMTM3AEntry\n"
        f = open(filepath,'r+')
        lines = f.readlines() # read old content
        f.seek(0) # go back to the beginning of the file
        f.write(GoodTag) # write new content at the beginning
        for line in lines: # write old content after new
            f.write(line)
        f.close()




    except Exception as e:
        print("An error occured.",e, "\n", traceback.format_exc())  



    
        
    
class YMLM3A_Import_Handler(bpy.types.Operator, ImportHelper):
    """Imports animation data from UMVC3 YML files"""
    bl_idname = ("screen.yml_import")
    bl_label = ("UMVC3 YML Import")
    bl_options = {'UNDO'}
    
    #Filters out non .yml files.
    filename_ext = ".yml"
    filter_glob: bpy.props.StringProperty(default="*.yml", options={'HIDDEN'})
    files: bpy.props.CollectionProperty(type=bpy.types.OperatorFileListElement)
    
    
    def execute(self, context):
        keywords = self.as_keywords(ignore=("filter_glob","files",))
        time_start = time.time()
        readM3AanimationData(self, context, **keywords)
        context.view_layer.update()
        time_end = time.time()
        print("Import done in ", time_end - time_start, " seconds.")
        return {"FINISHED"}
    
    #Checks if object selected is an armature.
    @classmethod
    def poll(self, context):
        if context.active_object is not None:
            if (context.active_object.type == 'ARMATURE'):
                return True
        return False    
            
class YMLM3A_Export_Handler(bpy.types.Operator, ExportHelper):
    """Exports animation data to LMT files"""
    bl_idname = ("screen.yml_export")
    bl_label = ("UMVC3 YML Export")
    
    #Filters out non .lmt files.
    filename_ext = ".yml"
    filter_glob: bpy.props.StringProperty(default="*.yml", options={'HIDDEN'}, maxlen=255)
    files: bpy.props.CollectionProperty(type=bpy.types.OperatorFileListElement)

    read_LoopFrame: bpy.props.IntProperty(
        name = "Loop Frame",
        description="The frame to loop the animation. Set to -1 to not loop it, as most animations have this set to -1.",
        default=-1,
    )

    def execute(self, context):
        keywords = self.as_keywords(ignore=("filter_glob","files"))
        time_start = time.time()
        #bpy.ops.export_scene.fbx(filepath='', check_existing=True)
        WriteM3AanimationData(context, self.filepath)
        #writeLMTAnimationData(self, context,self.filepath, self.read_lmtindex, self.read_hasuniquebonenames)
        context.view_layer.update()
        print("Script is Done.")
        return {"FINISHED"}
    
    #Checks if object selected is an armature.
    @classmethod
    def poll(self, context):
        if context.active_object is not None:
            if (context.active_object.type == 'ARMATURE'):
                return True
        return False    
            
#Menu Things. These allow the plugin to show up in the import menu.
def menu_func_import(self, context):
    self.layout.operator(YMLM3A_Import_Handler.bl_idname, text="Marvel 3 Anim YAML (.yml)")       
    
def menu_func_export(self, context):
    self.layout.operator(YMLM3A_Export_Handler.bl_idname, text="Marvel 3 Anim YAML (.yml)")    
    
def register():
    bpy.utils.register_class(YMLM3A_Import_Handler)
    bpy.types.TOPBAR_MT_file_import.append(menu_func_import)
    
    bpy.utils.register_class(YMLM3A_Export_Handler)
    bpy.types.TOPBAR_MT_file_export.append(menu_func_export)

def unregister():
    bpy.utils.unregister_class(YMLM3A_Import_Handler)
    bpy.types.TOPBAR_MT_file_import.remove(menu_func_import)
    
    bpy.utils.unregister_class(YMLM3A_Export_Handler)
    bpy.types.TOPBAR_MT_file_export.remove(menu_func_export)

if __name__ == "__main__":
    register()    