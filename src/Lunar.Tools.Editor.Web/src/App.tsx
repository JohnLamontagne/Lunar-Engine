import './index.css'
import { ProjectPanel } from './features/project/ProjectPanel'
import { ContentTree } from './features/content/ContentTree'
import { EditorArea } from './features/EditorArea'
import { DiagnosticsPanel } from './features/scripts/DiagnosticsPanel'
import { useEditorStore } from './store'

function Titlebar() {
  const { project, setProject, setContentTree } = useEditorStore()
  return (
    <div style={styles.titlebar}>
      <span style={styles.appName}>Lunar Tools Editor</span>
      {project && (
        <>
          <span style={styles.projectName}>{project.gameName}</span>
          <button
            style={{ marginLeft: 'auto', fontSize: 11 }}
            onClick={() => { setProject(null); setContentTree(null) }}
          >
            Close Project
          </button>
        </>
      )}
    </div>
  )
}

export default function App() {
  const { project } = useEditorStore()

  return (
    <div style={styles.shell}>
      <Titlebar />
      <div style={styles.body}>
        {project && <ContentTree />}
        <div style={styles.mainArea}>
          <EditorArea />
          <DiagnosticsPanel />
        </div>
      </div>
      <ProjectPanel />
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  shell: { height: '100%', display: 'flex', flexDirection: 'column', overflow: 'hidden' },
  titlebar: {
    height: 36,
    background: 'var(--bg-panel)',
    borderBottom: '1px solid var(--border)',
    display: 'flex',
    alignItems: 'center',
    padding: '0 12px',
    gap: 12,
    flexShrink: 0,
  },
  appName: { fontWeight: 600, color: 'var(--text-bright)', fontSize: 13 },
  projectName: { color: 'var(--text-dim)', fontSize: 12 },
  body: { flex: 1, display: 'flex', overflow: 'hidden' },
  mainArea: { flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' },
}
