# GitHub Copilot Instructions for RazzleDazzle! (Continued)

Welcome to the GitHub Copilot instructions for the RazzleDazzle! (Continued) mod for RimWorld. This mod introduces a range of performing arts to your colony, allowing colonists to write plays, compose music, and perform stand-up comedy. Below, you'll find an overview of the mod, its key features and systems, coding patterns and conventions, XML integration, and Harmony patching. Additionally, we'll provide suggestions for using GitHub Copilot to enhance your modding experience.

## Mod Overview and Purpose

RazzleDazzle! (Continued) is an updated version of the original Bar0ths mod, designed to bring a touch of entertainment and creativity to your RimWorld colony. The mod introduces four main performing arts:

1. **Theatre**: Colonists can write plays at sculpting benches and perform them for social interaction.
2. **Music**: Colonists can compose music at pianos and perform it for social interaction.
3. **Stand-up Comedy**: Colonists can rehearse and perform stand-up comedy at microphone stands for social interaction.
4. **Radio**: All performances are automatically broadcast if you have a broadcaster, with subscription payments sent in by nearby settlements based on the quality of your broadcast.

The mod aims to keep your colonists entertained and engaged, reducing their inclination to perform mundane tasks.

## Key Features and Systems

### Theatre

- **Writing Plays**: Colonists with the Artistic skill can write plays at sculpting benches.
- **Performing Plays**: Colonists with the Social skill can perform plays for social interaction.

### Music

- **Composing Music**: Colonists with the Artistic skill can compose music at pianos.
- **Performing Music**: Colonists with the Social skill can perform music for social interaction.

### Stand-up Comedy

- **Rehearsing Stand-up**: Colonists can rehearse stand-up comedy at microphone stands.
- **Performing Stand-up**: Colonists can perform stand-up comedy for social interaction.

### Radio

- **Broadcasting**: All performances are automatically broadcast from a broadcaster.
- **Subscriptions**: Nearby settlements subscribe to your broadcasts, paying based on the quality of your performance.
- **Goodwill**: Broadcasts can improve or reduce the goodwill of nearby settlements.

## Coding Patterns and Conventions

### C# Coding Standards

- **Namespaces**: Use `RazzleDazzle` as the base namespace for all classes and structs.
- **Naming Conventions**: Use PascalCase for class names and camelCase for method and variable names.
- **Comments**: Write clear and concise comments for complex logic and methods.

### XML Integration

- **Defs Files**: All mod-specific definitions are stored in XML files located in the `Defs` directory.
- **Def Types**: Use appropriate def types such as `JobDef`, `BuildingDef`, `TaleDef`, and `IncidentDef`.
- **Attributes**: Use XML attributes to define properties such as `defName`, `label`, `description`, and `category`.

### Harmony Patching

- **Patching**: Use Harmony to patch vanilla methods to add or modify functionality.
- **Patch Naming**: Name patches clearly and consistently, using the format `HarmonyPatch_{MethodName}`.
- **Annotations**: Use `[HarmonyPatch]` and `[HarmonyPostfix]` annotations to define patches.

## XML Integration

### Def Types

- **DutyDef**: Defines custom duties for performing arts.
- **TaleDef**: Defines custom tales for performance-related events.
- **IncidentDef**: Defines custom incidents for subscription payments.
- **JobDef**: Defines custom jobs for performing arts.
- **ThingCategoryDef**: Defines custom thing categories for performance-related items.

### Example XML Def

xml
<Def type="JobDef">
    <defName>RAZ_WritePlay</defName>
    <label>Write Play</label>
    <description>Colonists with the Artistic skill can write plays at sculpting benches.</description>
    <jobGiverClass>JobGiver_WritePlay</jobGiverClass>
    <workGiverDef>WorkGiver_WritePlay</workGiverDef>
    <jobCategory>JobCategory_Crafting</jobCategory>
</Def>


## Harmony Patching

### Example Harmony Patch

csharp
using HarmonyLib;
using Verse;

namespace RazzleDazzle
{
    [HarmonyPatch(typeof(Building), "GetInspectString")]
    public static class HarmonyPatch_Building_GetInspectString
    {
        [HarmonyPostfix]
        public static void Postfix(Building __instance, ref string __result)
        {
            if (__instance is Building_GrandPiano grandPiano)
            {
                __result += "\nQuality: " + grandPiano.BroadcastQualityString;
            }
        }
    }
}


## Suggestions for Copilot

### General Suggestions

- **Code Snippets**: Utilize Copilot to generate code snippets for new features, such as new job definitions or building classes.
- **Refactoring**: Use Copilot to assist with refactoring existing code to improve readability and performance.
- **Error Handling**: Use Copilot to generate error handling code for new methods and classes.

### XML Defs

- **Def Generation**: Use Copilot to generate XML def templates for new def types such as `JobDef` or `TaleDef`.
- **Attribute Completion**: Use Copilot to complete XML attributes and ensure they are correctly formatted.

### Harmony Patching

- **Patch Generation**: Use Copilot to generate Harmony patch templates for existing vanilla methods.
- **Patch Testing**: Use Copilot to assist with writing unit tests for patches to ensure they work as expected.

### Example Workflow

1. **Add a New Job Def**:
    - Open `JobDefs\JobDef_RAZ.xml`.
    - Use Copilot to generate a new `JobDef` template.
    - Fill in the necessary attributes and save the file.

2. **Create a New Building Class**:
    - Open `Building_GrandPiano.cs`.
    - Use Copilot to generate a new building class template.
    - Implement the necessary methods and properties.

3. **Patch a Vanilla Method**:
    - Open `HarmonyPatches.cs`.
    - Use Copilot to generate a new Harmony patch template.
    - Implement the patch logic and save the file.

By following these guidelines and utilizing GitHub Copilot, you can enhance your modding experience and create a more enjoyable and engaging RimWorld mod. Happy modding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

