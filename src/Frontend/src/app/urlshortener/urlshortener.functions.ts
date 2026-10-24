
/**
 * 
 * @param text The text string to copy to the clipboard
 * @returns A promise that resolves to true if successful, false otherwise
 */
export const copyToClipboard = async (text: string): Promise<boolean> => {
  if(!navigator.clipboard){
    console.warn("Clipboard API is not supported in this browser");
    return false;
  }

  try{
    await navigator.clipboard.writeText(text);
    return true;
  }
  catch(error){
    console.error("Failed to copy text to clipboard", error);
    return false;
  }
}