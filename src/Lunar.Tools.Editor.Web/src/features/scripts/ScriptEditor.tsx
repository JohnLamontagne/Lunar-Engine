import { useRef, useState } from 'react'
import Editor, { type OnMount } from '@monaco-editor/react'
import { api } from '../../api/client'
import { useEditorStore } from '../../store'

interface Props {
  filePath: string
}

export function ScriptEditor({ filePath }: Props) {
  const { openTabs, updateTabContent, markTabSaved, setDiagnostics } = useEditorStore()
  const [compiling, setCompiling] = useState(false)
  const [saving, setSaving] = useState(false)
  const editorRef = useRef<Parameters<OnMount>[0] | null>(null)

  const tab = openTabs.find((t) => t.filePath === filePath)
  if (!tab) return null

  // Capture non-nullable references so closures don't need re-narrowing
  const tabFilePath = tab.filePath
  const tabContent = tab.content
  const tabLabel = tab.label
  const tabDirty = tab.dirty

  async function handleSave() {
    if (saving) return
    setSaving(true)
    try {
      await api.scripts.save(tabFilePath, tabContent)
      markTabSaved(tabFilePath)
    } catch (e) {
      console.error('Save failed', e)
    } finally {
      setSaving(false)
    }
  }

  async function handleCompile() {
    setCompiling(true)
    try {
      setDiagnostics(await api.scripts.compile())
    } catch (e) {
      console.error('Compile failed', e)
    } finally {
      setCompiling(false)
    }
  }

  const handleMount: OnMount = (editor) => {
    editorRef.current = editor
    editor.addCommand(2097 /* Ctrl+S */, handleSave)
  }

  return (
    <div style={styles.container}>
      <div style={styles.toolbar}>
        <span style={styles.filename}>{tabLabel}{tabDirty ? ' ●' : ''}</span>
        <div style={{ flex: 1 }} />
        <button onClick={handleSave} disabled={!tabDirty || saving}>
          {saving ? 'Saving…' : 'Save'}
        </button>
        <button onClick={handleCompile} disabled={compiling} className="primary">
          {compiling ? 'Compiling…' : 'Compile'}
        </button>
      </div>
      <div style={styles.editorWrap}>
        <Editor
          height="100%"
          language="csharp"
          theme="vs-dark"
          value={tabContent}
          path={tabFilePath}
          onChange={(val) => updateTabContent(tabFilePath, val ?? '')}
          onMount={handleMount}
          options={{
            fontSize: 13,
            minimap: { enabled: false },
            scrollBeyondLastLine: false,
            wordWrap: 'off',
            renderLineHighlight: 'line',
            fontFamily: "'Cascadia Code', 'Fira Code', Consolas, monospace",
            fontLigatures: true,
          }}
        />
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: { flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' },
  toolbar: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
    padding: '4px 14px',
    borderBottom: '1px solid var(--border)',
    background: 'var(--bg-panel)',
    flexShrink: 0,
  },
  filename: { fontFamily: 'var(--font-mono)', fontSize: 12, color: 'var(--text-dim)' },
  editorWrap: { flex: 1, overflow: 'hidden' },
}
