# ConfuserEx Unpacker 2 (Supports Trinity & Netguard 4.5)

# STILL UNDER BETA

This is my own "mod" of this unpacker, ive added constant support for 2 parameter decryption support (Netguard 4.5, Ben Mhenni 4.5), and i added support for 3 parameters with string (Trinity). Added support for the packer in trinity and also added many different mutation removers that will come in handy for custom mods. You can easily clean programs piece by piece or however you want by editing the program.cs.
////////////////////////////////////////////////////////////

A new and updated version of my last unpacker for confuserex which people actually seem to use so i thought i would update it and actually make it better as that version is very poor

this is currently in beta and in its first version will only support confuserex with no modifications or additional options from confuserex itself. this will change as i add more features

this will heavily be based off my instruction emulator which makes it much more reliable as long as theres no hidden surprises from modified confuserex

i have not used sub modules due to making changes within de4dot.blocks in Int32/64Value i have modified the Shr_Un methods and such to fix a bug (well not really a bug but it prevents some operations from giving correct result) 

if you have an issue with this unpacker please make an issue report but if you simply go 

'does not work on this file please fix' i simply will just close your issue 

please make a detailed report and explain where it crashes 

Credits
TheProxy for his Reference Proxy Remover
Shadow Anti Tamper remover
0xd4d dnlib/de4dot
cawk confuserex-unpacker / mutation removers
