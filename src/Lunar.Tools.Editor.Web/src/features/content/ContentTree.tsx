import { useState } from 'react'
import { api, type ContentTreeNode } from '../../api/client'
import { useEditorStore } from '../../store'

const NODE_ICONS: Record<string, string> = {
  folder: '📁',
  map: '🗺',
  item: '🎒',
  npc: '👤',
  spell: '✨',
  anim: '🎬',
  dialogue: '💬',
  script: '📄',
}

interface TreeNodeProps {
  node: ContentTreeNode
  depth: number
}

function TreeNode({ node, depth }: TreeNodeProps) {
  const [expanded, setExpanded] = useState(depth < 2)
  const { openScriptTab, openItemTab, openSpellTab } = useEditorStore()
  const isFolder = node.nodeType === 'folder'
  const isOpenable = ['script', 'item', 'spell'].includes(node.nodeType)

  async function handleClick() {
    if (isFolder) { setExpanded((e) => !e); return }
    try {
      if (node.nodeType === 'script') {
        openScriptTab(await api.scripts.load(node.path))
      } else if (node.nodeType === 'item') {
        openItemTab(await api.items.load(node.path))
      } else if (node.nodeType === 'spell') {
        openSpellTab(await api.spells.load(node.path))
      }
    } catch (e) {
      console.error('Failed to open', node.nodeType, e)
    }
  }

  return (
    <div>
      <div
        style={{
          ...styles.row,
          paddingLeft: 8 + depth * 14,
          cursor: isFolder || isOpenable ? 'pointer' : 'default',
        }}
        onClick={handleClick}
      >
        <span style={styles.arrow}>
          {isFolder ? (expanded ? '▾' : '▸') : ' '}
        </span>
        <span style={styles.icon}>{NODE_ICONS[node.nodeType] ?? '•'}</span>
        <span style={styles.label}>{node.name}</span>
      </div>
      {isFolder && expanded && node.children.map((child) => (
        <TreeNode key={child.path} node={child} depth={depth + 1} />
      ))}
    </div>
  )
}

export function ContentTree() {
  const { contentTree, project } = useEditorStore()

  if (!project) return null

  return (
    <div style={styles.panel}>
      <div style={styles.header}>
        {project.gameName}
      </div>
      <div style={styles.tree}>
        {contentTree
          ? contentTree.children.map((node) => (
              <TreeNode key={node.path} node={node} depth={0} />
            ))
          : <div style={styles.empty}>No content</div>
        }
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  panel: {
    width: 240,
    minWidth: 180,
    maxWidth: 320,
    background: 'var(--bg-panel)',
    borderRight: '1px solid var(--border)',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden',
    userSelect: 'none',
  },
  header: {
    padding: '8px 12px',
    fontSize: 11,
    fontWeight: 600,
    textTransform: 'uppercase',
    letterSpacing: '0.6px',
    color: 'var(--text-dim)',
    borderBottom: '1px solid var(--border)',
  },
  tree: {
    flex: 1,
    overflowY: 'auto',
    padding: '4px 0',
  },
  row: {
    display: 'flex',
    alignItems: 'center',
    gap: 4,
    height: 22,
    padding: '0 8px',
    borderRadius: 3,
  },
  arrow: { width: 12, fontSize: 10, color: 'var(--text-dim)', flexShrink: 0 },
  icon: { fontSize: 12, flexShrink: 0 },
  label: { overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap', fontSize: 13 },
  empty: { padding: 12, color: 'var(--text-dim)', fontSize: 12 },
}
