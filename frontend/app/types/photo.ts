export type PhotoCategory = 'landscape-nature' | 'street-urban'

export interface Photo {
  id: string
  title: string
  category: PhotoCategory
  description: string
  shootDate: string
  visible: boolean
  order: number
  thumbnailUrl: string
  displayUrl: string
  createdAt: string
}
