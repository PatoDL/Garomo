#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class AkExternalSourceInfo : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkExternalSourceInfo(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkExternalSourceInfo obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkExternalSourceInfo() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkExternalSourceInfo(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public AkExternalSourceInfo() : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_0(), true) {
  }

  public AkExternalSourceInfo(global::System.IntPtr in_pInMemory, uint in_uiMemorySize, uint in_iExternalSrcCookie, uint in_idCodec) : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_1(in_pInMemory, in_uiMemorySize, in_iExternalSrcCookie, in_idCodec), true) {
  }

  public AkExternalSourceInfo(string in_pszFileName, uint in_iExternalSrcCookie, uint in_idCodec) : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_2(in_pszFileName, in_iExternalSrcCookie, in_idCodec), true) {
  }

  public AkExternalSourceInfo(uint in_idFile, uint in_iExternalSrcCookie, uint in_idCodec) : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_3(in_idFile, in_iExternalSrcCookie, in_idCodec), true) {
  }

  public void Clear() { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_Clear(swigCPtr); }

  public void Clone(AkExternalSourceInfo other) { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_Clone(swigCPtr, AkExternalSourceInfo.getCPtr(other)); }

  public static int GetSizeOf() { return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_GetSizeOf(); }

  public uint iExternalSrcCookie { set { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_iExternalSrcCookie_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_iExternalSrcCookie_get(swigCPtr); } 
  }

  public uint idCodec { set { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idCodec_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idCodec_get(swigCPtr); } 
  }

  public string szFile { set { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_szFile_set(swigCPtr, value); }  get { return AkSoundEngine.StringFromIntPtrOSString(AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_szFile_get(swigCPtr)); } 
  }

  public global::System.IntPtr pInMemory { set { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_pInMemory_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_pInMemory_get(swigCPtr); }
  }

  public uint uiMemorySize { set { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_uiMemorySize_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_uiMemorySize_get(swigCPtr); } 
  }

  public uint idFile { set { AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idFile_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idFile_get(swigCPtr); } 
  }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.