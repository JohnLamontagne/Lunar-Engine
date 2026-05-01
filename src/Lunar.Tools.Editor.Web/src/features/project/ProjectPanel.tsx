import { useState } from 'react'
import { api } from '../../api/client'
import { useEditorStore } from '../../store'

export function ProjectPanel() {
  const { project, setProject, setContentTree } = useEditorStore()
  const [projectPath, setProjectPath] = useState('')
  const [serverPath, setServerPath] = useState('')
  const [clientPath, setClientPath] = useState('')
  const [mode, setMode] = useState<'open' | 'create'>('open')
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  async function handleOpen() {
    if (!projectPath.trim()) return
    setBusy(true)
    setError(null)
    try {
      const p = await api.project.open(projectPath.trim())
      setProject(p)
      const tree = await api.content.tree()
      setContentTree(tree)
    } catch (e) {
      setError(String(e))
    } finally {
      setBusy(false)
    }
  }

  async function handleCreate() {
    if (!projectPath.trim() || !serverPath.trim() || !clientPath.trim()) return
    setBusy(true)
    setError(null)
    try {
      const p = await api.project.create(projectPath.trim(), serverPath.trim(), clientPath.trim())
      setProject(p)
      const tree = await api.content.tree()
      setContentTree(tree)
    } catch (e) {
      setError(String(e))
    } finally {
      setBusy(false)
    }
  }

  if (project) return null

  return (
    <div style={styles.overlay}>
      <div style={styles.dialog}>
        <h2 style={styles.title}>Lunar Tools Editor</h2>

        <div style={styles.tabs}>
          <button
            style={{ ...styles.tab, ...(mode === 'open' ? styles.tabActive : {}) }}
            onClick={() => setMode('open')}
          >
            Open Project
          </button>
          <button
            style={{ ...styles.tab, ...(mode === 'create' ? styles.tabActive : {}) }}
            onClick={() => setMode('create')}
          >
            New Project
          </button>
        </div>

        <div style={styles.fields}>
          <label>.lproj file path</label>
          <input
            value={projectPath}
            onChange={(e) => setProjectPath(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && mode === 'open' && handleOpen()}
            placeholder="/Users/you/Projects/MyGame/MyGame.lproj"
          />

          {mode === 'create' && (
            <>
              <label style={{ marginTop: 12 }}>Server data path</label>
              <input
                value={serverPath}
                onChange={(e) => setServerPath(e.target.value)}
                placeholder="/path/to/server-data"
              />
              <label style={{ marginTop: 12 }}>Client data path</label>
              <input
                value={clientPath}
                onChange={(e) => setClientPath(e.target.value)}
                placeholder="/path/to/client-data"
              />
            </>
          )}
        </div>

        {error && <div style={styles.error}>{error}</div>}

        <div style={styles.actions}>
          <button
            className="primary"
            disabled={busy}
            onClick={mode === 'open' ? handleOpen : handleCreate}
          >
            {busy ? 'Loading…' : mode === 'open' ? 'Open' : 'Create'}
          </button>
        </div>
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  overlay: {
    position: 'fixed', inset: 0,
    display: 'flex', alignItems: 'center', justifyContent: 'center',
    background: 'rgba(0,0,0,0.6)',
    zIndex: 100,
  },
  dialog: {
    background: 'var(--bg-panel)',
    border: '1px solid var(--border)',
    borderRadius: 6,
    padding: 24,
    width: 480,
    display: 'flex',
    flexDirection: 'column',
    gap: 16,
  },
  title: {
    fontSize: 16,
    fontWeight: 500,
    color: 'var(--text-bright)',
  },
  tabs: { display: 'flex', gap: 4 },
  tab: { background: 'transparent', color: 'var(--text-dim)', padding: '4px 12px' },
  tabActive: { background: 'var(--bg-active)', color: 'var(--text-bright)' },
  fields: { display: 'flex', flexDirection: 'column', gap: 4 },
  error: {
    color: 'var(--error)',
    fontSize: 12,
    fontFamily: 'var(--font-mono)',
    background: 'rgba(244,71,71,0.1)',
    border: '1px solid var(--error)',
    borderRadius: 3,
    padding: '6px 10px',
  },
  actions: { display: 'flex', justifyContent: 'flex-end' },
}
