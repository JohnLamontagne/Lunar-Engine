const BASE_URL = '/api'

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  })
  if (!res.ok) {
    const text = await res.text()
    throw new Error(`${res.status} ${res.statusText}: ${text}`)
  }
  if (res.status === 204) return undefined as T
  return res.json() as Promise<T>
}

export const api = {
  project: {
    get: () => request<ProjectManifest | null>('/project'),
    open: (projectFilePath: string) =>
      request<ProjectManifest>('/project/open', {
        method: 'POST',
        body: JSON.stringify({ projectFilePath }),
      }),
    create: (projectFilePath: string, serverDataPath: string, clientDataPath: string) =>
      request<ProjectManifest>('/project/create', {
        method: 'POST',
        body: JSON.stringify({ projectFilePath, serverDataPath, clientDataPath }),
      }),
  },
  content: {
    tree: () => request<ContentTreeNode>('/content/tree'),
  },
  scripts: {
    load: (path: string) =>
      request<ScriptDocument>(`/scripts/load?path=${encodeURIComponent(path)}`),
    save: (filePath: string, content: string) =>
      request<void>('/scripts/save', {
        method: 'POST',
        body: JSON.stringify({ filePath, content }),
      }),
    compile: () =>
      request<ValidationIssue[]>('/scripts/compile', { method: 'POST' }),
  },
  items: {
    load: (path: string) =>
      request<ItemEditorDocument>(`/items/load?path=${encodeURIComponent(path)}`),
    save: (doc: ItemEditorDocument) =>
      request<void>('/items/save', { method: 'POST', body: JSON.stringify(doc) }),
    create: (dirPath: string, name: string) =>
      request<ItemEditorDocument>('/items/create', {
        method: 'POST',
        body: JSON.stringify({ dirPath, name }),
      }),
    delete: (path: string) =>
      request<void>(`/items?path=${encodeURIComponent(path)}`, { method: 'DELETE' }),
  },
  spells: {
    load: (path: string) =>
      request<SpellEditorDocument>(`/spells/load?path=${encodeURIComponent(path)}`),
    save: (doc: SpellEditorDocument) =>
      request<void>('/spells/save', { method: 'POST', body: JSON.stringify(doc) }),
    create: (dirPath: string, name: string) =>
      request<SpellEditorDocument>('/spells/create', {
        method: 'POST',
        body: JSON.stringify({ dirPath, name }),
      }),
    delete: (path: string) =>
      request<void>(`/spells?path=${encodeURIComponent(path)}`, { method: 'DELETE' }),
  },
}

export interface ProjectManifest {
  projectFilePath: string
  serverDataPath: string
  clientDataPath: string
  gameName: string
}

export interface ContentTreeNode {
  name: string
  path: string
  nodeType: 'folder' | 'map' | 'item' | 'npc' | 'spell' | 'anim' | 'dialogue' | 'script'
  children: ContentTreeNode[]
}

export interface ScriptDocument {
  filePath: string
  relativePath: string
  content: string
}

export interface StatsDocument {
  strength: number
  intelligence: number
  dexterity: number
  defense: number
  vitality: number
}

export interface ItemEditorDocument {
  filePath: string
  name: string
  spriteName: string
  stackable: boolean
  itemType: 'Equipment' | 'Usable' | 'NA'
  slotType: 'NE' | 'MainArm' | 'SideArm' | 'Ring' | 'SecRing' | 'Helm' | 'Boots' | 'Chest' | 'Legs' | 'Shoulder'
  strength: number
  intelligence: number
  dexterity: number
  defence: number
  health: number
  behaviorKey: string
}

export interface SpellEditorDocument {
  filePath: string
  name: string
  displaySpriteName: string
  castTime: number
  activeTime: number
  cooldownTime: number
  healthCost: number
  manaCost: number
  casterAnimationPath: string
  targetAnimationPath: string
  statModifiers: StatsDocument
  statRequirements: StatsDocument
  behaviorKey: string
}

export interface ValidationIssue {
  fileName: string
  filePath: string | null
  line: number
  column: number
  diagnosticId: string
  message: string
  severity: 'error' | 'warning' | 'info'
}
