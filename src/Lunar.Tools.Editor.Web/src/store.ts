import { create } from 'zustand'
import type {
  ContentTreeNode,
  ItemEditorDocument,
  ProjectManifest,
  ScriptDocument,
  SpellEditorDocument,
  ValidationIssue,
} from './api/client'

export type TabNodeType = 'script' | 'item' | 'spell'

export interface OpenTab {
  filePath: string
  relativePath: string
  label: string
  /** Raw JSON string for item/spell; source text for scripts. */
  content: string
  nodeType: TabNodeType
  dirty: boolean
}

interface EditorStore {
  project: ProjectManifest | null
  contentTree: ContentTreeNode | null
  openTabs: OpenTab[]
  activeTabPath: string | null
  diagnostics: ValidationIssue[]

  setProject: (p: ProjectManifest | null) => void
  setContentTree: (tree: ContentTreeNode | null) => void

  openScriptTab: (doc: ScriptDocument) => void
  openItemTab: (doc: ItemEditorDocument) => void
  openSpellTab: (doc: SpellEditorDocument) => void
  closeTab: (filePath: string) => void
  setActiveTab: (filePath: string) => void
  updateTabContent: (filePath: string, content: string) => void
  markTabSaved: (filePath: string) => void

  setDiagnostics: (issues: ValidationIssue[]) => void
}

function labelFor(filePath: string): string {
  return filePath.split(/[\\/]/).pop() ?? filePath
}

function addTab(
  get: () => EditorStore,
  set: (partial: Partial<EditorStore> | ((s: EditorStore) => Partial<EditorStore>)) => void,
  tab: OpenTab,
) {
  const existing = get().openTabs.find((t) => t.filePath === tab.filePath)
  if (existing) { set({ activeTabPath: tab.filePath }); return }
  set((s) => ({ openTabs: [...s.openTabs, tab], activeTabPath: tab.filePath }))
}

export const useEditorStore = create<EditorStore>((set, get) => ({
  project: null,
  contentTree: null,
  openTabs: [],
  activeTabPath: null,
  diagnostics: [],

  setProject: (p) => set({ project: p }),
  setContentTree: (tree) => set({ contentTree: tree }),

  openScriptTab: (doc) =>
    addTab(get, set, {
      filePath: doc.filePath,
      relativePath: doc.relativePath,
      label: labelFor(doc.filePath),
      content: doc.content,
      nodeType: 'script',
      dirty: false,
    }),

  openItemTab: (doc) =>
    addTab(get, set, {
      filePath: doc.filePath,
      relativePath: doc.filePath,
      label: labelFor(doc.filePath),
      content: JSON.stringify(doc, null, 2),
      nodeType: 'item',
      dirty: false,
    }),

  openSpellTab: (doc) =>
    addTab(get, set, {
      filePath: doc.filePath,
      relativePath: doc.filePath,
      label: labelFor(doc.filePath),
      content: JSON.stringify(doc, null, 2),
      nodeType: 'spell',
      dirty: false,
    }),

  closeTab: (filePath) => {
    set((s) => {
      const remaining = s.openTabs.filter((t) => t.filePath !== filePath)
      const next =
        s.activeTabPath === filePath
          ? (remaining.at(-1)?.filePath ?? null)
          : s.activeTabPath
      return { openTabs: remaining, activeTabPath: next }
    })
  },

  setActiveTab: (filePath) => set({ activeTabPath: filePath }),

  updateTabContent: (filePath, content) =>
    set((s) => ({
      openTabs: s.openTabs.map((t) =>
        t.filePath === filePath ? { ...t, content, dirty: true } : t,
      ),
    })),

  markTabSaved: (filePath) =>
    set((s) => ({
      openTabs: s.openTabs.map((t) =>
        t.filePath === filePath ? { ...t, dirty: false } : t,
      ),
    })),

  setDiagnostics: (issues) => set({ diagnostics: issues }),
}))
